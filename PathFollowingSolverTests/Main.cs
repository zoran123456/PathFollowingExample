using PathFollowingUI;
using System;
using Xunit;

namespace PathFollowingSolverTests
{
    public class Main
    {
        [Fact]
        public void Constructor_NoParams_Throws()
        {
            
            Assert.Throws<ArgumentException>(() => new PathFollowingSolver(null, null));

        }

        [Fact]
        public void Constructor_FirstParamInvalid_Throws()
        {

            Assert.Throws<ArgumentException>(() => new PathFollowingSolver("", null));

        }

        [Fact]
        public void Constructor_SecondParamInvalid_Throws()
        {

            Assert.Throws<ArgumentException>(() => new PathFollowingSolver("Boards/board1.txt", ""));

        }

        [Fact]
        public void Constructor_BoardInvalid_Throws()
        {

            Assert.Throws<ArgumentException>(() => new PathFollowingSolver("Boards/invalidBoard.txt", "ABCD"));

        }

        [Fact]
        public void Solver_Board1_Returns_Expected_Result()
        {
            var solver = new PathFollowingSolver("Boards/board1.txt", "ACB");

            string word;
            string path;

            var ok = solver.Solve(out word, out path);


            Assert.True(ok);
            Assert.Equal("ACB", word);
            Assert.Equal("@---A---+|C|+---+|+-B-x", path);
        }

        [Fact]
        public void Solver_Board2_Returns_Expected_Result()
        {
            var solver = new PathFollowingSolver("Boards/board2.txt", "ABCD");

            string word;
            string path;

            var ok = solver.Solve(out word, out path);


            Assert.True(ok);
            Assert.Equal("ABCD", word);
            Assert.Equal("@|A+---B--+|+----C|-||+---D--+|x", path);
        }

        [Fact]
        public void Solver_Board3_Returns_Expected_Result()
        {
            var solver = new PathFollowingSolver("Boards/board3.txt", "BEEFCAKE");

            string word;
            string path;

            var ok = solver.Solve(out word, out path);


            Assert.True(ok);
            Assert.Equal("BEEFCAKE", word);
            Assert.Equal("@---+B||E--+|E|+--F--+|C|||A--|-----K|||+--E--Ex", path);
        }

        [Fact]
        public void Solver_ComplexBoard_Returns_Expected_Result()
        {
            var solver = new PathFollowingSolver("Boards/complexBoard.txt", "ABCDEFG");

            string word;
            string path;

            var ok = solver.Solve(out word, out path);

            Assert.True(ok);
            Assert.Equal("ABCDEFG", word);
        }
    }
}
