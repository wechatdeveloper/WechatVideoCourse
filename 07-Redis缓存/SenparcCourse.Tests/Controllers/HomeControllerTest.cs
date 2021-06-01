using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SenparcCourse;
using SenparcCourse.Controllers;

namespace SenparcCourse.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void About()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            Assert.AreEqual("Your application description page.", result.ViewBag.Message);
        }

        [TestMethod]
        public void Contact()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Contact() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        private int totalThread = 100; //计划线程要执行20次
        private int finishedThread = 0;

        [TestMethod]
        public void LockTest()
        { 
            //开启多个线程
            for (int i = 0; i < 100; i++)
            { 
                Thread thread = new Thread(RunSingleLockTest);
                thread.Start(); 
            }

            while (finishedThread!= totalThread)
            {
                //等待线程执行到100次
            }

            Console.WriteLine("线程执行完毕:"+totalThread.ToString());
         
        }
         
        private void RunSingleLockTest()
        {  
            HomeController controller = new HomeController();

            ContentResult result = controller.LockTest() as ContentResult;

            Console.WriteLine(result.Content);

            finishedThread++; //记得线程执行的次数
        }


    }
}
