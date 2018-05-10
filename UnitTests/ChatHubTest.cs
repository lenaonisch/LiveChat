using System;
using LiveChat;
using LiveChat.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TypeMock.ArrangeActAssert;

namespace UnitTests
{
    [TestClass]
    public class ChatHubTest
    {
        ChatHub chatHub;

        [TestInitialize]
        public void Init()
        {
            chatHub = new ChatHub();
        }

        [TestMethod]
        public void TestSend_IncorrectMessage()
        {
            Isolate.WhenCalled(() => chatHub.Send(""));
            Assert.ThrowsException<ChatHubException>(()=>chatHub.Send(""));
        }

        [TestMethod]
        public void TestSend_IncorrectMessageCorrectGroup()
        {
            Isolate.WhenCalled(() => chatHub.Send("1", ""));
            Assert.ThrowsException<ChatHubException>(() => chatHub.Send("1", ""));
        }

        [TestMethod]
        public void TestSend_CorrectMessageIncorrectGroup()
        {
            Isolate.WhenCalled(() => chatHub.Send("", "message"));
            Assert.ThrowsException<ChatHubException>(() => chatHub.Send("", "message"));
        }

        [TestMethod]
        public void TestSend_CorrectMessageIncorrectGroup2()
        {
            Isolate.WhenCalled(() => chatHub.Send("a", "message"));
            Assert.ThrowsException<ChatHubException>(() => chatHub.Send("a", "message"));
        }

        [TestMethod]
        public void TestSend_CorrectMessageIncorrectGroup3()
        {
            Isolate.WhenCalled(() => chatHub.Send("1a", "message"));
            Assert.ThrowsException<ChatHubException>(() => chatHub.Send("1a", "message"));
        }

        [TestMethod]
        public void TestSend_CorrectMessageCorrectGroup()
        {
            Isolate.WhenCalled(() => chatHub.Send("1", "message"));
            chatHub.Send("1", "message");
        }
    }
}
