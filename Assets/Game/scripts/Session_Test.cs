using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raider.Game;
using Raider.Game.Saves.System;
using Raider.Test.Saves.System;

namespace Raider.Test
{
    [TestClass]
    public class Session_Test
    {
        [TestMethod]
        public void SystemSaveData()
        {
            ISystemSaveDataHandler systemSaveDataHandler;
            SystemSaveDataStructure systemSaveData;

            systemSaveDataHandler = new MockSystemSaveDataHandler();
            systemSaveData = new SystemSaveDataStructure();

            systemSaveData.sessionToken = "test";

            systemSaveDataHandler.SaveData(systemSaveData);

            Assert.AreEqual("test",systemSaveDataHandler.GetData().sessionToken);
        }
    }
}
