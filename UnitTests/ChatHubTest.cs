using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using LiveChat;
using LiveChat.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.ChatModels;
using TypeMock.ArrangeActAssert;
using Microsoft.CSharp;
using System.Threading;
using System.Collections.Generic;

namespace UnitTests
{
    public class DummyClass
    {

        public async Task addNewMessageToPage(string name, string group, string message)
        {
            return;
        }
    }

    [TestClass]
    public class ChatHubTest
    {
        ChatHub chatHub;

        [TestInitialize]
        public void Init()
        {
            chatHub = Isolate.Fake.Instance<ChatHub>(Members.CallOriginal); //new ChatHub();
            Isolate.Fake.StaticMethods(typeof(StaticData), Members.ReturnRecursiveFakes);
            var company = new Company("c1") { ID = 1 };
            StaticData.AddCompany(company);
            chatHub.Context = new Microsoft.AspNet.SignalR.Hubs.HubCallerContext(null, "connectstr1");
            StaticData.Users.TryAdd("connectstr1", new UserProfile() { BaseUser = new BaseUser("userName", company) });
            StaticData.Groups.TryAdd(company, new ConcurrentDictionary<string, Chat>());
            StaticData.Groups[company].TryAdd("0", new Chat() { Messages = new List<Message>() });
            Isolate.WhenCalled(() => chatHub.Clients.Group("0")).DoInstead((s) => { return new DummyClass(); });
        }

        [TestMethod]
        public void TestSend_IncorrectMessage()
        {
            Isolate.WhenCalled(() => chatHub.Send(""));
            Assert.ThrowsException<ChatHubException>(() => chatHub.Send(""));
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
            Assert.ThrowsException<FormatException>(() => chatHub.Send("", "message"));
        }

        [TestMethod]
        public void TestSend_CorrectMessageIncorrectGroup2()
        {
            Isolate.WhenCalled(() => chatHub.Send("a", "message"));
            Assert.ThrowsException<FormatException>(() => chatHub.Send("a", "message"));
        }

        [TestMethod]
        public void TestSend_CorrectMessageIncorrectGroup3()
        {
            Isolate.WhenCalled(() => chatHub.Send("1a", "message"));
            Assert.ThrowsException<FormatException>(() => chatHub.Send("1a", "message"));
        }

        [TestMethod]
        public void TestSend_CorrectMessageIncorrectGroup4()
        {
            Isolate.WhenCalled(() => chatHub.Send("10000000000000000000000000000000000000000000000000000000000000", "message"));
            Assert.ThrowsException<OverflowException>(() => chatHub.Send("10000000000000000000000000000000000000000000000000000000000000", "message"));
        }

        [TestMethod]
        public void TestSend_CorrectMessageCorrectGroup()
        {
            Isolate.WhenCalled(() => chatHub.Send("0", "message"));
            chatHub.Send("0", "message");
        }
    }
}
