using System;
using NUnit.Framework;

namespace BattleDiceTests
{
	[TestFixture]
	public class TestTimeManager
	{
		private TimeManager _timeManager;

		private TimeProviderForUnitTests _timeProvider = new TimeProviderForUnitTests();

		[SetUp]
		public void SetUp()
		{
			TimeManager.Reset();
			_timeManager = NonUnitySingleton<TimeManager>.instance;
			_timeManager.TimeProvider = _timeProvider;
			TimeManager.CLIENT_TIME_LAG = 0L;
			_timeProvider.CurrentDeviceTime = 0L;
			TimeManager.ServerTime = 0L;
		}

		[Test]
		public void TestTimeManagerSingleton()
		{
			TimeManager instance = NonUnitySingleton<TimeManager>.instance;
			Assert.AreSame(_timeManager, instance);
		}

		[Test]
		public void TestGetTimeDelta()
		{
			_timeProvider.CurrentDeviceTime = 1000L;
			long timeDelta = _timeManager.GetTimeDelta(2000L);
			long expected = 1000L;
			Assert.AreEqual(expected, timeDelta, "GetTimeDelta method failed. ");
			timeDelta = _timeManager.GetTimeDelta(500L);
			expected = -500L;
			Assert.AreEqual(expected, timeDelta, "GetTimeDelta method failed. ");
			timeDelta = _timeManager.GetTimeDelta(1000L);
			expected = 0L;
			Assert.AreEqual(expected, timeDelta, "GetTimeDelta method failed. ");
		}

		[Test]
		public void TestTimestampDirection()
		{
			_timeProvider.CurrentDeviceTime = 2000L;
			Assert.IsFalse(_timeManager.IsTimestampInFuture(1000L), "0:A timestamp from one second ago was incorrectly identified as being in the future.");
			Assert.IsTrue(_timeManager.IsTimestampInPast(1000L), "1:A timestamp from one second ago was incorrectly identified as being in the future.");
			_timeProvider.CurrentDeviceTime = 2000L;
			Assert.IsTrue(_timeManager.IsTimestampInFuture(3000L), "2:A timestamp one second in the future was incorrectly identified as being in the past.");
			Assert.IsFalse(_timeManager.IsTimestampInPast(3000L), "3:A timestamp one second in the future was incorrectly identified as being in the past.");
		}

		[Test]
		public void TestEvent()
		{
			bool firstEventOccurred = false;
			bool secondEventOccurred = false;
			long num = 10000L;
			long preTimestamp = num - 1000;
			long postTimestamp = num + 1000;
			Action PreEventHandler = delegate
			{
				firstEventOccurred = true;
				_timeManager.UnRegisterForEvent(preTimestamp, PreEventHandler);
			};
			Action PostEventHandler = delegate
			{
				secondEventOccurred = true;
				_timeManager.UnRegisterForEvent(postTimestamp, PostEventHandler);
			};
			_timeProvider.CurrentDeviceTime = num;
			_timeManager.RegisterForEvent(preTimestamp, PreEventHandler);
			_timeManager.RegisterForEvent(postTimestamp, PostEventHandler);
			_timeManager.CheckEvents();
			Assert.IsTrue(firstEventOccurred, "The PreEventHandler was not invoked when it should have been.");
			Assert.IsFalse(secondEventOccurred, "The PostEventHandler was invoked whenit should not have been.");
			firstEventOccurred = false;
			_timeManager.CheckEvents();
			Assert.IsFalse(firstEventOccurred, "The PreEventHandler should have been unregistered, but it was invoked.");
			_timeProvider.CurrentDeviceTime = postTimestamp + 100;
			_timeManager.CheckEvents();
			Assert.IsTrue(secondEventOccurred, "The PostEventHandler should have been invoked, but it wasn't");
		}
	}
}
