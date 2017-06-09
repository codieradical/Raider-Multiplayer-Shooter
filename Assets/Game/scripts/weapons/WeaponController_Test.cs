using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Raider.Game.Weapons;

namespace Raider.Test.Weapons
{
    [TestClass]
    public class WeaponController_Test
    {
        [TestMethod]
        public void WeaponCustomizationAmmoInitialization()
        {
            //Arrange
            WeaponController weaponController = new WeaponController();
            WeaponSettings weaponCustomization = new WeaponSettings();
            weaponCustomization.maxAmmo = 20;
            weaponCustomization.clipSize = 10;

            //Act
            weaponController.totalAmmo = weaponCustomization.maxAmmo;
            weaponController.clipAmmo = weaponCustomization.clipSize;

            //Assert
            Assert.AreEqual(weaponCustomization.maxAmmo, weaponController.totalAmmo);
            Assert.AreEqual(weaponCustomization.clipSize, weaponController.clipAmmo);
        }

        [TestMethod]
        public void Shoot()
        {
            WeaponController weaponController = new WeaponController();
            weaponController.clipAmmo = 10;
            weaponController.totalAmmo = 20;

            int shotCount = 0;

            while(weaponController.clipAmmo > 0)
            {
                weaponController.clipAmmo--;
                shotCount++;
            }

            Assert.AreEqual(weaponController.clipAmmo, shotCount);
        }

        [TestMethod]
        public void Reload()
        { 
            WeaponController weaponController = new WeaponController();
            WeaponSettings weaponCustomization = new WeaponSettings();
            weaponCustomization.maxAmmo = 20;
            weaponCustomization.clipSize = 10;
            weaponController.totalAmmo = weaponCustomization.maxAmmo;
            weaponController.clipAmmo = 5;

            if (weaponController.totalAmmo >= weaponCustomization.clipSize)
                weaponController.clipAmmo = weaponCustomization.clipSize;
            else
                weaponController.clipAmmo = weaponController.totalAmmo;

            weaponController.totalAmmo -= weaponController.clipAmmo;

            Assert.AreEqual(weaponCustomization.clipSize, weaponController.clipAmmo);
            Assert.AreEqual(15, weaponController.totalAmmo);
        }
    }
}
