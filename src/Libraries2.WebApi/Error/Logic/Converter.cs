using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xlent.Lever.Libraries2.Core.Assert;
using Xlent.Lever.Libraries2.Core.Error.Logic;
using Xlent.Lever.Libraries2.Core.Error.Model;

namespace Xlent.Lever.Libraries2.WebApi.Error.Logic
{
    /// <summary>
    /// This class has conversion methods for converting between unsuccessful HTTP responses and Fulcrum exceptions.
    /// Fulcrum is only using three HTTP status codes for errors; 400, 500 and 503.
    /// This was based on the following blog article http://blog.restcase.com/rest-api-error-codes-101/
    /// </summary>
    public class Converter
    {
        private static readonly Dictionary<string, Func<string, Exception, FulcrumException>> FactoryMethodsCache = new Dictionary<string, Func<string, Exception, FulcrumException>>();
        private static readonly Dictionary<string, HttpStatusCode> HttpStatusCodesCache = new Dictionary<string, HttpStatusCode>();

        /// <summary>
        /// Use this method to add a new <see cref="FulcrumException"/>. This means that it will be included in converting.
        /// </summary>
        /// <param name="fulcrumExceptionType">The type of the exception.</param>
        /// <param name="statusCode">The status code that it should be converted to if we convert it to an HTTP response.</param>
        public static void AddFulcrumException(Type fulcrumExceptionType, HttpStatusCode? statusCode = null)
        {
            InternalContract.RequireNotNull(fulcrumExceptionType, nameof(fulcrumExceptionType));
            var methodInfo = fulcrumExceptionType.GetMethod("Create");
            Func<string, Exception, FulcrumException> createInstanceDelegate;
            try
            {
                createInstanceDelegate =
                    (Func<string, Exception, FulcrumException>)Delegate.CreateDelegate(
                        typeof(Func<string, Exception, FulcrumException>), methodInfo);
            }
            catch (Exception e)
            {
                throw new FulcrumContractException(
                    $"The type {fulcrumExceptionType.FullName} must have a factory method Create(string message, Exception innerException).",
                    e);
            }
            // ReSharper disable once PossibleNullReferenceException
            // ReSharper disable once RedundantCast
            var exception = createInstanceDelegate("test", (Exception)null);
            FactoryMethodsCache.Add(exception.Type, createInstanceDelegate);
            if (statusCode != null) HttpStatusCodesCache.Add(exception.Type, statusCode.Value);
        }

        static Converter()
        {
            // Core
            AddFulcrumException(typeof(FulcrumAssertionFailedException), HttpStatusCode.InternalServerError);
            AddFulcrumException(typeof(FulcrumBusinessRuleException), HttpStatusCode.BadRequest);
            AddFulcrumException(typeof(FulcrumConflictException), HttpStatusCode.BadRequest);
            AddFulcrumException(typeof(FulcrumContractException));
            AddFulcrumException(typeof(FulcrumNotFoundException), HttpStatusCode.BadRequest);
            AddFulcrumException(typeof(FulcrumNotImplementedException), HttpStatusCode.InternalServerError);
            AddFulcrumException(typeof(FulcrumTryAgainException), HttpStatusCode.InternalServerError);

            // WebApi
            AddFulcrumException(typeof(FulcrumServiceContractException), HttpStatusCode.BadRequest);
            AddFulcrumException(typeof(FulcrumUnauthorizedException), HttpStatusCode.BadRequest);
            AddFulcrumException(typeof(FulcrumForbiddenAccessException), HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Convert an HTTP response (<paramref name="response"/>) into a <see cref="FulcrumException"/>.
        /// </summary>
        public static async Task<FulcrumException> ToFulcrumExceptionAsync(HttpResponseMessage response)
        {
            InternalContract.RequireNotNull(response, nameof(response));
            if (response.IsSuccessStatusCode) return null;
            if (response.Content == null) return null;
            await response.Content?.LoadIntoBufferAsync();
            var contentAsString = await response.Content?.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(contentAsString)) return null;
            var error = Parse<FulcrumError>(contentAsString);
            if (error?.Type == null) return null;
            ValidateStatusCode(response.StatusCode, error);
            var fulcrumException = ToFulcrumException(error);
            if (fulcrumException != null) return fulcrumException;
            var message = $"The Type ({error.Type}) was not recognized: {ToJsonString(error, Formatting.Indented)}";
            return new FulcrumAssertionFailedException(message, ToFulcrumException(error.InnerError));
        }

        /// <summary>
        /// Convert a <see cref="FulcrumError"/> (<paramref name="error"/>) into a <see cref="FulcrumException"/>.
        /// </summary>
        public static FulcrumException ToFulcrumException(FulcrumError error)
        {
            if (error == null) return null;
            var fulcrumException = CreateFulcrumException(error);
            fulcrumException.CopyFrom(error);
            return fulcrumException;
        }

        /// <summary>
        /// Convert an exception (<paramref name="e"/>) into an HTTP response message.
        /// </summary>
        public static HttpResponseMessage ToHttpResponseMessage(Exception e)
        {
            InternalContract.RequireNotNull(e, nameof(e));
            var error = ToFulcrumError(e);
            if (error == null)
            {
                var message = $"The exception {e.GetType().FullName} was not recognized as a Fulcrum Exception. Message: {e.Message}";
                e = new FulcrumAssertionFailedException(message, e);
                error = ToFulcrumError(e);
                if (error == null) return FatalErrorAsHttpResponse(e);
            }
            var statusCode = ToHttpStatusCode(error);
            if (statusCode == null)
            {
                var message =
                    $"The Type of the following error could not be converted to an HTTP status code: {ToJsonString(error, Formatting.Indented)}.";
                e = new FulcrumAssertionFailedException(message, e);
                error = ToFulcrumError(e);
                if (error == null) return FatalErrorAsHttpResponse(e);
                statusCode = ToHttpStatusCode(error);
                if (statusCode == null) return FatalErrorAsHttpResponse(e);
            }
            var content = ToJsonString(error, Formatting.Indented);
            var stringContent = new StringContent(content, Encoding.UTF8);
            var response = new HttpResponseMessage(statusCode.Value)
            {
                Content = stringContent
            };
            return response;
        }

        private static HttpResponseMessage FatalErrorAsHttpResponse(Exception e)
        {
            var message =
            $"FATAL error in Xlent.Lever.Libraries.WebApi.GlobalExceptionHandler. Could not convert an exception to an HTTP response message. Additional information: {e.Message}";
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(message,Encoding.UTF8)
            };
            return response;
        }

        /// <summary>
        /// Convert an exception (<paramref name="e"/>) into a <see cref="FulcrumError"/>.
        /// </summary>
        public static FulcrumError ToFulcrumError(Exception e)
        {
            var fulcrumException = e as FulcrumException;
            if (fulcrumException == null) return null;
            var error = new FulcrumError();
            error.CopyFrom(fulcrumException);
            error.InnerError = ToFulcrumError(fulcrumException.InnerException);
            return error;
        }

        /// <summary>
        /// Transform a <see cref="FulcrumException"/> (<paramref name="source"/>) into a new <see cref="FulcrumException"/>.
        /// </summary>
        /// <param name="source">The exception to transform.</param>
        /// <param name="serverTechnicalName">The server that the </param>
        /// <returns>The same <paramref name="source"/> unless it was an exception that needs to be transformed; then a new exception is returned.</returns>
        /// <remarks>If <paramref name="source"/> had a null ServerTechnicalName it will be set to <paramref name="serverTechnicalName"/> as a side effect.</remarks>
        public static FulcrumException FromServiceToBll(FulcrumException source, string serverTechnicalName = null)
        {
            if (source == null) return null;
            source.ServerTechnicalName = source.ServerTechnicalName ?? serverTechnicalName;
            switch (source.Type)
            {
                case FulcrumAssertionFailedException.ExceptionType:
                case FulcrumNotImplementedException.ExceptionType:
                    return new FulcrumAssertionFailedException($"Did not expect {source.ServerTechnicalName ?? "server"} to return the following error: {source.Message}", source);
                case FulcrumServiceContractException.ExceptionType:
                    return new FulcrumAssertionFailedException($"Bad call to { source.ServerTechnicalName ?? "Server" }: { source.Message}", source);
                case FulcrumUnauthorizedException.ExceptionType:
                    return new FulcrumAssertionFailedException($"Unauthorized call to {source.ServerTechnicalName ?? "server"}: {source.Message}", source);
                default:
                    return source;
            }
        }

        private static void ValidateStatusCode(HttpStatusCode statusCode, FulcrumError error)
        {
            var expectedStatusCode = ToHttpStatusCode(error);
            if (expectedStatusCode == null)
            {
                throw new FulcrumAssertionFailedException(
                    $"The Type of the content could not be converted to an HTTP status code: {ToJsonString(error, Formatting.Indented)}.");
            }
            if (expectedStatusCode != statusCode)
            {
                throw new FulcrumAssertionFailedException(
                    $"The HTTP error response had status code {statusCode}, but was expected to have {expectedStatusCode.Value}, due to the Type in the content: \"{ToJsonString(error, Formatting.Indented)}");
            }
        }

        private static FulcrumException CreateFulcrumException(FulcrumError error, bool okIfNotExists = false)
        {
            if (!FactoryMethodsCache.ContainsKey(error.Type))
            {
                if (okIfNotExists) return null;
                var message = $"The Type ({error.Type}) was not recognized: {ToJsonString(error, Formatting.Indented)}. It must be added to {typeof(Converter).FullName}.";
                return new FulcrumAssertionFailedException(message, ToFulcrumException(error.InnerError));
            }
            var factoryMethod = FactoryMethodsCache[error.Type];
            var fulcrumException = factoryMethod(error.TechnicalMessage, ToFulcrumException(error.InnerError));
            return fulcrumException;
        }

        /// <summary>
        /// Checks a dictionary for the proper <see cref="HttpStatusCode"/> for <paramref name="fulcrumException"/>.
        /// </summary>
        public static HttpStatusCode? ToHttpStatusCode(FulcrumException fulcrumException)
        {
            var error = ToFulcrumError(fulcrumException);
            return ToHttpStatusCode(error);
        }

        /// <summary>
        /// Checks a dictionary for the proper <see cref="HttpStatusCode"/> for <paramref name="error"/>.
        /// </summary>
        public static HttpStatusCode? ToHttpStatusCode(FulcrumError error)
        {
            if (error == null) return null;
            if (!HttpStatusCodesCache.ContainsKey(error.Type)) return null;
            return HttpStatusCodesCache[error.Type];
        }

        /// <summary>
        /// Converts <paramref name="fulcrumError"/> into a JSON string.
        /// </summary>
        /// <returns></returns>
        private static string ToJsonString(IFulcrumError fulcrumError, Formatting formatting)
        {
            return JObject.FromObject(fulcrumError).ToString(formatting);
        }

        /// <summary>
        /// Parse a JSON string and converts it into an object.
        /// </summary>
        /// <param name="jsonObject"></param>
        /// <returns></returns>
        public static T Parse<T>(string jsonObject)
            where T : class
        {
            if (jsonObject == null) return null;
            try
            {
                var json = JObject.Parse(jsonObject);
                return json.ToObject<T>();
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
