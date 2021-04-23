using Dms.Cms.SystemModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartSUB_Remote_Monitor.Model;
using SmartSUB_Remote_Monitor.Services;
using SmartSUB_Remote_Monitor.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dms.Cms.DatabaseManager;
using Dms.Cms.Messaging;

namespace SmartSUB_Remote_Monitor.UnitTests
{
    internal class Mock_SystemInfo : ISystemInfo
    {
        //public mock_systeminfo()
        //{

        //}

        public NodeID NodeIDFromLogicalName(string logicalName)
        {
            return mock_logicalNameToNodeDefinition[logicalName].ID;
        }

        public bool IsFullSystemModel
        {
            get { return true; }
        }

        private uint siteID1 = 1;
        private List<UInt16> listOfSiteIDs = new List<UInt16>(1);

        public IEnumerable<NodeDefinition> RootNodes { get; }
        public IEnumerable<NodeDefinition> AllSystemNodes { get; }
        public IEnumerable<UInt16> SiteIds
        {
            get { return listOfSiteIDs; }
        }

        public NodeDefinition NodeFromLogicalName(string logicalName)
        {
            return mock_logicalNameToNodeDefinition[logicalName];
        }
        public NodeType TypeFromID(Symbol id)
        {
            return mock_nodeTypeIdToNodeType[id];
        }
        public NodeDefinition NodeFromID(NodeID id)
        {
            return mock_nodeIDToNodeDefinition[id];
        }
        public bool NodeIdInUse(NodeID id)
        {
            throw new NotImplementedException();
        }
        public bool LogicalNameInUse(string logicalName)
        {
            throw new NotImplementedException();
        }
        public bool TypeIdInUse(Symbol id)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<NodeDefinition> NodesByType(NodeType type)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<NodeDefinition> NodesByGroupID(Symbol groupID)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<NodeType> NodeTypes { get; }

        public NodeDefinition mock_addNodeDefinition(NodeType type, string logicalName, NodeDefinition parent, NodeID nodeID)
        {
            NodeDefinition nodeDefinition = NodeDefinition.CreateNew(type, logicalName, null, nodeID);
            mock_logicalNameToNodeDefinition.Add(logicalName, nodeDefinition);
            mock_nodeIDToNodeDefinition.Add(nodeID, nodeDefinition);

            if (!mock_nodeTypeIdToNodeType.ContainsKey(type.ID))
            {
                mock_nodeTypeIdToNodeType.Add(type.ID, type);
            }

            if (parent != null)
            {
                NodeDefinition.AddChild(parent, nodeDefinition);
            }

            return nodeDefinition;
        }

        public void mock_addType(NodeType type)
        {
            if (!mock_nodeTypeIdToNodeType.ContainsKey(type.ID))
            {
                mock_nodeTypeIdToNodeType.Add(type.ID, type);
            }
        }

        public void mock_removeNodeDefinition(NodeDefinition node)
        {
            mock_logicalNameToNodeDefinition.Remove(node.LogicalName);
            mock_nodeIDToNodeDefinition.Remove(node.ID);
            mock_nodeTypeIdToNodeType.Remove(node.Type.ID);
        }

        public Dictionary<string, NodeDefinition> mock_logicalNameToNodeDefinition = new Dictionary<string, NodeDefinition>();
        public Dictionary<NodeID, NodeDefinition> mock_nodeIDToNodeDefinition = new Dictionary<NodeID, NodeDefinition>();
        public Dictionary<Symbol, NodeType> mock_nodeTypeIdToNodeType = new Dictionary<Symbol, NodeType>();
    }

    public class Mock_INodeController : INodeController
    {
        public uint sendNodeCommandCallCount = 0;
        public Message sendNodeCommandLastCommand;
        public void EnableNode(NodeID nodeID, DateTime timestamp)
        {
            throw new NotImplementedException();
        }

        public void DisableNode(NodeID nodeID, DateTime timestamp)
        {
            throw new NotImplementedException();
        }

        public void SetNodeProperties(NodeID nodeID, DateTime timestamp, Message properties)
        {
            throw new NotImplementedException();
        }

        public void SetNodeAlarmState(AlarmRecord alarmRecord)
        {
            throw new NotImplementedException();
        }

        public void SendNodeCommand(NodeID nodeID, DateTime timestamp, Message command, SharedPipe callback)
        {
            ++sendNodeCommandCallCount;
            sendNodeCommandLastCommand = command;

            string bob = command.ToString();
            //var subscriptionHandler =
            //    (GlobalState.SystemInterface.SubscriptionHandler as Mock_SubscriptionHandler);
            //var systemInfo = (GlobalState.SystemInterface.SystemInfo as Mock_SystemInfo);
            //if (subscriptionHandler == null || systemInfo == null)
            //{
            //    throw new NotImplementedException();
            //}

            //var newNodeId = new NodeID(4);
            //systemInfo.mock_addNodeDefinition(systemInfo.TypeFromID(Symbol.Intern("Qdnp3Plugin")),
            //    "other.Qdnp3Plugin",
            //    systemInfo.NodeFromLogicalName("other"),
            //    newNodeId);
            //subscriptionHandler.systemInfoListenerOnNodeAdded(systemInfo.NodeFromLogicalName("other").ID,
            //    newNodeId);
        }
    }

    internal class Mock_SystemInterface : ISystemInterface
    {
        public Mock_SystemInterface(ISystemInfo systemInfo, INodeController nodeController)
        {
            SystemInfo = systemInfo;
            SystemDateTime = NullDateTime.Instance;
            NodeController = nodeController;
        }

        public INodeListener NodeListener { get; }
        public INodeController NodeController { get; }
        public ISubscriptionHandler SubscriptionHandler { get; }
        public ISystemInfo SystemInfo { get; }
        public ILoginInfo LoginInfo { get; }
        public ISystemInfoListener SystemInfoListener { get; }
        public IDateTime SystemDateTime { get; set; }

    }

    [TestClass]
    public class StationsTests
    {
        [TestMethod]
        public void UnitTestExample_ExpectedIsBob_ReturnsTrue()
        {
            //Arrange
            var station = new Stations();

            //Act
            bool result = station.UnitTestExample("Bob");

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GetNumActiveAlarmsFromSmartSUB_CheckFunctionCallIncrements()
        {
            //Arrange
            var Mock_SystemInfo = new Mock_SystemInfo();
            var Mock_NodeController = new Mock_INodeController();
            var mockSystemInterface = new Mock_SystemInterface(Mock_SystemInfo, Mock_NodeController);
            var station = new Stations();
            NodeType nodeType = new NodeType(Symbol.Intern("TestType1"), false, false, new List<PropertyDefinition>());
            Mock_SystemInfo.mock_addNodeDefinition(nodeType, "DatabaseManager", null, new NodeID(0, 1));

            //Act
            uint actualCallCount = Mock_NodeController.sendNodeCommandCallCount;
            uint expectedCallCount = 0;

            //Assert
            Assert.AreEqual(expectedCallCount, actualCallCount);

            //Act
            station.GetNumActiveAlarmsFromSmartSUB(mockSystemInterface, 1);
            expectedCallCount = 1;
            actualCallCount = Mock_NodeController.sendNodeCommandCallCount;
            string expectedMessage = Mock_NodeController.sendNodeCommandLastCommand.ToString();
            string actualMessage =
                "GetRecords ( NodeID = NodeID ( NodeID : uint32 = 0x0001ffff ), UpdateKind : uint8 = 0x03, Record = ActiveAlarm ( ) )";

            //Assert
            Assert.AreEqual(expectedCallCount, actualCallCount);
            Assert.AreEqual(expectedMessage, actualMessage);
        }

        [TestMethod]
        public void GetStations_Test()
        {
            //Arrange
            var Mock_SystemInfo = new Mock_SystemInfo();
            var Mock_NodeController = new Mock_INodeController();
            var mockSystemInterface = new Mock_SystemInterface(Mock_SystemInfo, Mock_NodeController);
            var station = new Stations();
            ObservableCollection<Stations> stations = new ObservableCollection<Stations>();

            //Act
            stations = station.GetStations(mockSystemInterface);

            //Assert
        }

        [TestMethod]
        public void GetDistinctStations_Test()
        {
            //Arrange
            var Mock_SystemInfo = new Mock_SystemInfo();
            var Mock_NodeController = new Mock_INodeController();
            var mockSystemInterface = new Mock_SystemInterface(Mock_SystemInfo, Mock_NodeController);
            var station = new Stations();
            ObservableCollection<Stations> stations = new ObservableCollection<Stations>();

            //Act
            station.GetDistinctStations(mockSystemInterface);

            //Assert
        }

        [TestMethod]
        public void ExtractNumAlarms_Test()
        {
            var Mock_SystemInfo = new Mock_SystemInfo();

            NodeType dbtype = new NodeType(Symbol.Intern("DatabaseManager"), false, false, new List<PropertyDefinition>());
            Mock_SystemInfo.mock_addNodeDefinition(dbtype, "DatabaseManager", null, new NodeID(1));

            var Mock_NodeController = new Mock_INodeController();
            var mockSystemInterface = new Mock_SystemInterface(Mock_SystemInfo, Mock_NodeController);
            var station = new Stations();

            var alarmList = new List<AlarmRecord>();
            var alarmTime1 = DateTime.Now;
            var alarmRecord1 = new AlarmRecord(new NodeID(0, 1), Symbol.Intern("TestAlarm1"), alarmTime1, true);

            alarmList.Add(alarmRecord1);

            station.ExtractNumAlarms(QueryResult.Valid, alarmList, 1, mockSystemInterface);
        }
    }
}
