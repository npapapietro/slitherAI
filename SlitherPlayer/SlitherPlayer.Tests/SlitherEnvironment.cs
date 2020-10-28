using System;
using Xunit;
using Moq;
using OpenQA.Selenium;
using System.Drawing;
using SlitherPlayer.Environment;
using System.Linq;
using System.Collections.Generic;

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

        [Fact]
        public void TestCoordinates()
        {
            int R = 100;
            int n = 4;
            var coord = new Coordinates(n, R);

           
            var angles = new double[] { 0, Math.PI / 2, Math.PI,  3 * Math.PI / 2};
            var xy = new List<(int, int)>();
            foreach(var t in angles)
            {
                xy.Add((Convert.ToInt32(Math.Cos(t) * R), - Convert.ToInt32(Math.Sin(t) * R)));
            }

            Assert.True(xy.All(coord.PositionMapping.Contains));
        }
    }
}
