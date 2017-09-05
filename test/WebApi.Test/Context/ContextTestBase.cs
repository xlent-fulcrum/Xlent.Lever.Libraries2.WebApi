using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xlent.Lever.Libraries2.Core.Context;

namespace Xlent.Lever.Libraries2.WebApi.Test.Context
{
    public abstract class ContextTestBase
    {
        private IValueProvider _provider;

        protected abstract IValueProvider CreateValueProvider();

        private readonly object _lockObject = new object();
        private string _latestSet;


        [TestInitialize]
        public void Init()
        {
            _provider = CreateValueProvider();
        }

        [TestMethod]
        public void GetNotInitialized()
        {
            Assert.IsNull(_provider.GetValue<string>("X"));
        }

        [TestMethod]
        public async Task TaskCanReadTop()
        {
            const string topString = "LocalString";
            SetValue(_provider, topString);
            var task = GetValueAsync(_provider);
            var value = await task;
            Assert.AreEqual(topString, value);
        }

        [TestMethod]
        public async Task TaskCanUpdateTop()
        {
            const string topString = "LocalString1";
            const string taskString = "TaskString";
            SetValue(_provider, topString);
            await SetValueAsync(_provider, taskString);
            Assert.AreEqual(taskString, GetValue(_provider));
        }

        [TestMethod]
        public async Task TaskCanUpdateTopConfigureAwait()
        {
            const string topString = "LocalString";
            const string taskString = "TaskString";
            SetValue(_provider, topString);
            await SetValueAsync(_provider, taskString).ConfigureAwait(false);
            Assert.AreEqual(taskString, GetValue(_provider));
        }

        [TestMethod]
        public async Task UNEXPECTED_ValueCreatedByTaskDoesNotReachTop()
        {
            const string taskString = "TaskString";
            // Same as above except we comment out the row below
            // SetValue(provider, topString);
            await SetValueAsync(_provider, taskString);
            Assert.AreEqual(taskString, _latestSet);
            Assert.IsNull(GetValue(_provider));
        }

        [TestMethod]
        public async Task TopUpdateAffectsTask()
        {
            const string localString1 = "LocalString1";
            const string localString2 = "LocalString2";
            const string taskString = "TaskString";
            SetValue(_provider, localString1);
            var task = CanSurviveContextChange(_provider, taskString);
            await Task.Delay(10);
            Assert.AreEqual(taskString, _latestSet);
            SetValue(_provider, localString2);
            var confirmed = await task;
            Assert.AreEqual(localString2, _latestSet);
            Assert.IsFalse(confirmed);
            Assert.AreEqual(localString2, GetValue(_provider));
        }

        [TestMethod]
        public async Task TwoAsyncTaskUpdateCollides()
        {
            const string topString = "LocalString";
            const string string1 = "String 1";
            const string string2 = "String 2";
            SetValue(_provider, topString);
            var separateContexts = await SetValueInTwoTasksAsync(string1, string2);
            Assert.AreEqual(string2, GetValue(_provider));
            Assert.IsFalse(separateContexts);
        }

        /// <summary>
        /// An async task is affected by a change in the caller
        /// </summary>
        [TestMethod]
        public async Task UNEXPECTED_TwoAsyncTaskCreateDoesNotCollide()
        {
            const string string1 = "String 1";
            const string string2 = "String 2";
            var separateContexts = await SetValueInTwoTasksAsync(string1, string2);
            Assert.IsNull(GetValue(_provider));
            Assert.IsTrue(separateContexts);
        }

        /// <summary>
        /// The threads does not affect each other because they are on different contexts.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void TwoConcurrentThreadsHasDifferentCreateContexts()
        {
            //const string stringA = "String A";
            //const string stringB = "String B";

            var threads = new List<Thread>();
            for (var i = 0; i < 25; i++)
            {
                threads.Add(new Thread(() =>
                {
                    var value = $"x{i}";
                    SetValue(_provider, value);
                    Assert.AreEqual(value, GetValue(_provider));
                }));
            }
            threads.ForEach(x => x.Start());


            //    var t1 = new Thread(async () =>
            //    {
            //        Assert.IsTrue(await CanSurviveContextChange(_provider, stringA));
            //        Assert.AreEqual(stringA, _latestSet);
            //    });
            //    t1.Start();
            //    t1.Join();
            //    Assert.AreEqual(stringA, GetValue(_provider));
        }

        /// <summary>
        /// The threads does not affect each other because they are on different contexts.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task UNEXPECTED_TwoConcurrentThreadsHasSameUpdateContexts()
        {
            const string stringA = "String A";
            const string stringB = "String B";
            SetValue(_provider, "InitialValue");
            var t1 = Task.Run(async () => Assert.IsFalse(await CanSurviveContextChange(_provider, stringA)));
            await Task.Delay(10);
            Assert.AreEqual(stringA, _latestSet);
            var t2 = Task.Run(() => SetValue(_provider, stringB));
            await Task.Delay(10);
            Assert.AreEqual(stringB, _latestSet);
            await Task.WhenAll(t1, t2);
        }

        /// <summary>
        /// Top level and sub thread have different contexts.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task SeparateThreadCanAccessTop()
        {
            const string topString = "LocalString";
            SetValue(_provider, topString);
            await Task.Run(() => Assert.AreEqual(topString, GetValue(_provider)));
        }

        /// <summary>
        /// Top level and sub thread have different contexts.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task UNEXPECTED_SeparateThreadCanUpdateTop()
        {
            const string topString = "TopString";
            const string threadString = "ThreadString";
            SetValue(_provider, topString);
            await Task.Run(() => SetValue(_provider, threadString));
            Assert.AreEqual(threadString, _latestSet);
            Assert.AreEqual(threadString, GetValue(_provider));
        }

        [TestMethod]
        public void TwoProvidersHaveTheSameContext()
        {
            const string stringA = "String A";
            const string stringB = "String B";
            SetValue(_provider, stringA);
            var provider2 = CreateValueProvider();
            Assert.AreEqual(stringA, GetValue(_provider));
            SetValue(provider2, stringB);
            Assert.AreEqual(stringB, GetValue(provider2));
            Assert.AreEqual(stringB, GetValue(_provider));
        }

        private async Task<bool> SetValueInTwoTasksAsync(string string1, string string2)
        {
            var task = CanSurviveContextChange(_provider, string1);
            await Task.Delay(10);
            Assert.AreEqual(string1, _latestSet);
            await SetValueAsync(_provider, string2);
            Assert.AreEqual(string2, _latestSet);
            var confirmed = await task;
            return confirmed;
        }

        private async Task<bool> CanSurviveContextChange(IValueProvider provider, string localValue)
        {
            SetValue(provider, localValue);
            var count = 0;
            while (count < 50 && _latestSet == localValue)
            {
                await Task.Delay(10);
                count++;
            }
            return _latestSet != localValue && localValue == GetValue(provider);
        }

        private async Task SetValueAsync(IValueProvider provider, string value)
        {
            SetValue(provider, value);
            await Task.Yield();
        }

        private void SetValue(IValueProvider provider, string value)
        {
            lock (_lockObject)
            {
                provider.SetValue("X", value);
                _latestSet = value;
            }
        }

        private static async Task<string> GetValueAsync(IValueProvider provider)
        {
            return await Task.FromResult(provider.GetValue<string>("X"));
        }

        private static string GetValue(IValueProvider provider)
        {
            return provider.GetValue<string>("X");
        }
    }
}