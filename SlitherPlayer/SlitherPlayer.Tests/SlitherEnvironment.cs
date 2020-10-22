using System;
using Xunit;
using Moq;
using OpenQA.Selenium;
using System.Drawing;
using SlitherPlayer.Environment;

namespace SlitherPlayer.Tests
{
    public class TestSlitherEnvironment
    {

        [Fact]
        public void TestGetCenter()
        {
            var driver = new Mock<IWebDriver>();
            var canvas = new Mock<IWebElement>();
            canvas.Setup(x => x.Size).Returns(new Size { Height = 50, Width = 50 });
            driver.Setup(x => x.FindElement(It.IsAny<By>())).Returns(canvas.Object);

            var result = new SlitherEnvironment().GetCenter(driver.Object, out var x, out var y);

            Assert.True(result);
            Assert.Equal(25, x);
            Assert.Equal(25, y);

        }
    }
}
