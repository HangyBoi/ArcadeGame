using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace GXPEngine
{
    public enum Shape
    {
        RED,
        BLUE,
        GREEN,
        YELLOW,

        LIGHTNING,
        BOMB
    }
    public static class MagicShape
    {
        /* Symbols explained:
         * 
         * Q = up left
         * W = up
         * E = up right
         * D = right
         * C = down right
         * X = down
         * Z = down left
         * A = left
         * 
         */


        public static string playerInput = "";
        public static Direction prevInput;

        public delegate void Spell(Shape shape);
        public static event Spell CastSpell;

        public static Dictionary<Direction, char> directionList = new Dictionary<Direction, char>()
        {
            {Direction.UP_LEFT, 'Q'},
            {Direction.UP, 'W'},
            {Direction.UP_RIGHT, 'E'},
            {Direction.RIGHT, 'D'},
            {Direction.DOWN_RIGHT, 'C'},
            {Direction.DOWN, 'X'},
            {Direction.DOWN_LEFT, 'Z'},
            {Direction.LEFT, 'A'},
        };
        public static Dictionary<Shape, List<string>> shapeList = new Dictionary<Shape, List<string>>()
        {
            {Shape.RED, new List<string>()
            {
                "E","WE","WQ","QW","EW",
                "X","XC","XZ","CX","ZX"
            }},

            {Shape.BLUE, new List<string>()
            {
                "D","DE","DC","ED","CD",
                "A","AQ","AZ","QA","ZA"
            }},

            {Shape.GREEN, new List<string>()
            {
                "CE","CEW","CWE","CW",
                "XE","XWE","XW",
                "ZQ","ZWQ","ZQW","ZW",
                "XQ","XWQ",
                "XCE","CXE","CXW",
                "XZQ","ZXQ",
                "ZAQ","CDE"
            }},


            {Shape.YELLOW, new List<string>()
            {
                "EC","EWC","WEC","WC",
                "EX","EWX","WEX","WX",
                "QZ","WQZ","QWZ","WZ",
                "QX","WQX","QWX",
                "EXC","ECX","WXC","WCX",
                "QXZ","QZX","WZX","WXZ",
                "QAZ","EDC"
            }},

            {Shape.LIGHTNING, new List<string>()
            {
                "ZDZ","XDZ","XDX","ZDX","ZDA","ADZ","ADA",
                "ZXDX","XZDX","ZXDZ","XZDZ","XZDA","ZXDA",
                "XDZX","XDXZ","ZDZX","ZDXZ","ADZX","ADXZ",
                "XZDZX","ZXDXZ","ZXDZX","XZDXZ",

                "ZEZ","XEZ","XEX","ZEX",
                "ZXEX","XZEX","ZXEZ","XZEZ",
                "XEZX","XEXZ","ZEZX","ZEXZ",
                "XZEZX","ZXEXZ","ZXEZX","XZEXZ",

                "ZEDZ","XEDZ","XEDX","ZEDX",
                "ZXEDX","XZEDX","ZXEDZ","XZEDZ",
                "XEDZX","XEDXZ","ZEDZX","ZEDXZ",
                "XZEDZX","ZXEDXZ","ZXEDZX","XZEDXZ",

                "ZDEZ","XDEZ","XDEX","ZDEX",
                "ZXDEX","XZDEX","ZXDEZ","XZDEZ",
                "XDEZX","XDEXZ","ZDEZX","ZDEXZ",
                "XZDEZX","ZXDEXZ","ZXDEZX","XZDEXZ",


                "EAE","WAE","WAW","EAW","EAD","WAD","DAW","DAD",
                "EWAW","WEAW","EWAE","WEAE","EWAD","WEAD",
                "WAEW","WAWE","EAEW","EAWE","DAEW","DAWE",
                "WEAEW","EWAWE","EWAEW","WEAWE",

                "EZE","WZE","WZW","EZW",
                "EWZW","WEZW","EWZE","WEZE",
                "WZEW","WZWE","EZEW","EZWE",
                "WEZEW","EWZWE","EWZEW","WEZWE",

                "EZAE","WZAE","WZAW","EZAW",
                "EWZAW","WEZAW","EWZAE","WEZAE",
                "WZAEW","WZAWE","EZAEW","EZAWE",
                "WEZAEW","EWZAWE","EWZAEW","WEZAWE",

                "EAZE","WAZE","WAZW","EAZW",
                "EWAZW","WEAZW","EWAZE","WEAZE",
                "WAZEW","WAZWE","EAZEW","EAZWE",
                "WEAZEW","EWAZWE","EWAZEW","WEAZWE",

            }},

            {Shape.BOMB, new List<string>()
            {
                "XDW","CDW","ZDW","XDQ","XDE",
                "XCW","XEW","XEQ","ZCW",

                "XDEQ","XDCW","XDCE",
                "XEDW","XCDE","XEDQ","CEDW","ZCDW",

                "XCDW","CXDW","XZDW","ZXDW",
                "XCDQ","CXDQ","XZDQ","ZXDQ",
                "XDQW","XDWQ","XDWE","XDEW",
                "ZDQW","ZDWQ","ZDWE","ZDEW",

                "XCDEW",

                "XAW","CAW","ZAW","XAQ","XAE",
                "XZW","XQW","XQE","CZW",

                "XAZQ","XAZW","XAQE",
                "XQAW","XZAQ","XQAE","CZAW","ZQAW",

                "XCAW","CXAW","XZAW","ZXAW",
                "XCAE","CXAE","XZAE","ZXAE",
                "XAQW","XAWQ","XAWE","XAEW",
                "CAQW","CAWQ","CAWE","CAEW",

                "WEDCX"
            }},

            
        };
        public static Dictionary<string, Shape> uniqueShapeList = new Dictionary<string, Shape>();

        public static Bitmap[] spellSprite;
        public static void LoadSprites()
        {
            spellSprite = new Bitmap[]
            {
                new Bitmap("Assets\\spells\\red.png"),
                new Bitmap("Assets\\spells\\blue.png"),
                new Bitmap("Assets\\spells\\green.png"),
                new Bitmap("Assets\\spells\\yellow.png"),
                new Bitmap("Assets\\spells\\lightning.png"),
                new Bitmap("Assets\\spells\\bomb.png")
            };
        }
        public static void AddStroke(Direction dir)
        {
            if (ArduinoTracker.D[4] != 3)
                return;
            if (dir == prevInput)
                return;
            playerInput += directionList[dir];
            Console.WriteLine(playerInput);
            prevInput = dir;
        }
        public static void ClearStroke()
        {
            playerInput = "";
            prevInput = Direction.NONE;
        }
        public static void SpellAttempt()
        {
            if (uniqueShapeList.ContainsKey(playerInput))
            {
                CastSpell?.Invoke(uniqueShapeList[playerInput]);
                Console.WriteLine(uniqueShapeList[playerInput]);
            }
            ClearStroke();
        }
        // DEBUG ONLY !!!
        public static void SpellPerform(Shape sh)
        {
            CastSpell?.Invoke(sh);
        }
        public static void FillShapeList()
        {
            uniqueShapeList.Clear();
            foreach (Shape sh in shapeList.Keys)
            {
                foreach(string st in shapeList[sh])
                {
                    uniqueShapeList.Add(st, sh);
                }
            }
        }
    }
}
