using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using server.Drivers;
using server.Drivers.Kernel;

namespace server.Tests
{
    // [TestFixture]
    public class DriverTest
    {
        // [Test(Description = "Test loading kernel driver on specific machine")]
        public static void TestLoading() {
            BridgeDriver.RegisterLoggerCallback(Target);

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            Console.WriteLine($"Test working directory is: {Directory.GetCurrentDirectory()}");

            var state = KernelDriverBridge.InitializeEnvironment();
            if (state == KernelDriverInitState.RhmsDriverManagerIncorrectDrvSignature) {
                Assert.Ignore("Won't check driver signature, target machine should disable 'Driver Signature Checking' " +
                              "before perform driver loading test, because it always leads to test failure");
            }

            Assert.AreEqual(KernelDriverInitState.RhmsDrvNoError, state, "Unnable to load driver or create service");
        }

        private static void Target(DriverLogLevel level, string message){
            Console.WriteLine($"{level} {message}");
        }
    }
}
