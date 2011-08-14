using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmeraldLibrary;
using System.IO;
using Microsoft.CSharp;
using System.Reflection;
using System.CodeDom.Compiler;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

namespace EmeraldDream
{
    public class Story
    {


        GameWindow game;

        Dictionary<string, Type> floortypes = new Dictionary<string, Type>();
        Dictionary<string, Type> objecttypes = new Dictionary<string, Type>();
        Dictionary<string, Map> maps = new Dictionary<string, Map>();
        Dictionary<string, MapObject> objects = new Dictionary<string, MapObject>();
        Dictionary<string, Image> images = new Dictionary<string, Image>();

        TileBank tb = null;

        string scriptpath;
        public Story(GameWindow g, string scriptpath)
        {
            this.game = g;
            this.tb = g.Tiles;
            this.scriptpath = scriptpath;

            // Grab images
            string[] imagefiles = Directory.GetFiles(Path.Combine(scriptpath, "images"), "*.*g");
            imagefiles.ToList().ForEach(x =>
            {
                string imagename = Path.GetFileName(x);
                images.Add(imagename, Image.FromFile(x));
            });


            // Grab tiles
            string[] files = Directory.GetFiles(Path.Combine(scriptpath, "tiles"), "*.png");
            files.ToList().ForEach(x =>
            {
                string tilename = Path.GetFileNameWithoutExtension(x);
                tb.LoadTile(tilename, x);
            });


            ObjectsAssembly fa = new ObjectsAssembly(tb);

            // Grab floor types
            string[] floors = Directory.GetFiles(Path.Combine(scriptpath, "floors"), "*.floor");
            floors.ToList().ForEach(x =>
            {
                string floorname = Path.GetFileNameWithoutExtension(x);
                fa.AddFloor(floorname, x);
            });

            // Grab object types
            string[] objects = Directory.GetFiles(Path.Combine(scriptpath, "objects"), "*.desc");
            objects.ToList().ForEach(x =>
            {
                string objname = Path.GetFileNameWithoutExtension(x);
                fa.AddObject(objname, x);
            });


            // Compile the types and prepare them
            fa.Compile();

            floortypes = fa.GetFloorTypes();
            objecttypes = fa.GetObjectTypes();


            // Grab maps
            string[] maps = Directory.GetDirectories(Path.Combine(scriptpath, "maps"));
            maps.ToList().ForEach(x =>
            {
                Map m = LoadMap(g.canvas,x);
                string mapname = Path.GetFileNameWithoutExtension(x);
                this.maps[mapname] = m;
            });

            currentscene = new PlayScene(null, null);
            g.Scene = currentscene;
            player = null;
            this.m = null;
            narrationdialog = new NarrationDialog(30, 400, 740, 200);
            menudialog = new MenuDialog(30, 400, 740, 200);
            staticImage = new StaticImage(images);

            currentscene.AddLayer(staticImage);
            currentscene.AddLayer(narrationdialog);
            currentscene.AddLayer(menudialog);

            //menudialog.SetQuestion("どうする？");
            //menudialog.AddMenuItem("a", "Run");
            //menudialog.AddMenuItem("b", "Pokemon");
            //menudialog.AddMenuItem("c", "LOL What?!");
            //menudialog.Visible = true;
        }

        Map LoadMap(GameControl g, string mapdir)
        {
            Map m;
            BinaryFormatter bf = new BinaryFormatter();

            // Load the floor
            using (StreamReader floortiles = new StreamReader(Path.Combine(mapdir, "floor.tiles")))
            {
                using (FileStream floormap = new FileStream(Path.Combine(mapdir, "floor.map"), FileMode.Open))
                {
                    int[,] floor = (int[,])bf.Deserialize(floormap);
                    int w = floor.GetUpperBound(0) + 1;
                    int h = floor.GetUpperBound(1) + 1;
                    m = new Map(g, w, h);
                    
                    Dictionary<int, string> tilenumToName = new Dictionary<int, string>();
                    while (!floortiles.EndOfStream)
                    {
                        string line = floortiles.ReadLine();
                        string[] tokens = line.Split(':');
                        tilenumToName[int.Parse(tokens[0])] = tokens[1];
                    }
                    for (int xx = 0; xx < w; xx++)
                    {
                        for (int yy = 0; yy < h; yy++)
                        {
                            m.SetFloor((MapFloor)Activator.CreateInstance(floortypes[tilenumToName[floor[xx, yy]]]), xx, yy);
                        }
                    }
                }
            }

            Regex objectnamelabel = new Regex(@"^\[(?<objectname>[a-z][a-z|0-9]*)\]", RegexOptions.Compiled);
            // Load the objects that are on this map
            using (StreamReader objectlist = new StreamReader(Path.Combine(mapdir, "objects.list")))
            {
                string line = objectlist.ReadLine();
                Match match = objectnamelabel.Match(line);

                while (!objectlist.EndOfStream)
                {
                    string instancename = match.Groups["objectname"].ToString();
                    ICharacter mo = null;
                    // For each instance block
                    while (!objectlist.EndOfStream)
                    {
                        line = objectlist.ReadLine();

                        if (line.StartsWith("type:"))
                        {
                            string type = line.Substring(5);
                            mo = (ICharacter)Activator.CreateInstance(objecttypes[type]);
                        }
                        else if (line.StartsWith("location:"))
                        {
                            string[] tokens = line.Substring(9).Split(',');
                            m.SetCharacter(mo, int.Parse(tokens[0]), int.Parse(tokens[1]));
                        }
                        else if (line.StartsWith("use:"))
                        {
                            Script s = new Script("inst_" + instancename, objectlist, true);

                            mo.Use = (st) => { s.Execute(st); };
                        }

                        match = objectnamelabel.Match(line);
                        if (match.Success) break;
                    }
                }

                //var ob = (ICharacter)Activator.CreateInstance(objecttypes["box"]);
                //m.SetCharacter(ob, 10, 5);

                //Thread t = new Thread(x =>
                //{
                //    int xx = 10;
                //    int yy = 5;
                //    bool right = true;
                //    while (true)
                //    {
                //        Thread.Sleep(1000);

                //        if (right)
                //        {
                //            if (m.MoveCharacter(xx, yy, Direction.Right))
                //            {
                //                xx++;
                //            }
                //            else
                //            {
                //                right = false;
                //            }
                //        }
                //        else
                //        {
                //            if (m.MoveCharacter(xx, yy, Direction.Left))
                //            {
                //                xx--;
                //            }
                //            else
                //            {
                //                right = true;
                //            }
                //        }
                //    }
                //});
                //t.IsBackground = true;
                //t.Start();

            }

            return m;
        }

        // Actions
        // These are methods that can be called by script programs to perform tasks
        public void LoadNew()
        {
            // The script path has all the resource and script files needed for the story.
            // First we read the main script
            Script main = new Script("MainScript", "res/main.script");
            main.Execute(this);

        }

        public void SetImage(string image)
        {
            if (image == "nothing")
            {
                staticImage.Image = null;
            }
            else
            {
                staticImage.Image = image;
            } 

        }

        public void ChangeMap(string mapName)
        {
            DeactivatePlayerControl();
            m = maps[mapName];
            currentscene.ChangeMap(m);
            ActivatePlayerControl();
        }

        public void ChangeCharacter(string character)
        {
            DeactivatePlayerControl();
            player = (ICharacter)objects[character];
            m.SetCharacter(player, m.CameraX, m.CameraY);
            currentscene.ChangeCharacter(player);
            ActivatePlayerControl();
        }

        public MapObject CreateObjectInstance(string obj, string name)
        {
            MapObject m = (MapObject)Activator.CreateInstance(objecttypes[obj]);
            objects[name] = m;
            return m;
        }

        public void ActivatePlayerControl()
        {
            currentscene.DXKeyDown += new EventHandler<KeyEventArgs>(currentscene_DXKeyDown);
            currentscene.KeyDown += new EventHandler<KeyEventArgs>(currentscene_KeyDown);
        }

        public void DeactivatePlayerControl()
        {
            currentscene.DXKeyDown -= new EventHandler<KeyEventArgs>(currentscene_DXKeyDown);
            currentscene.KeyDown -= new EventHandler<KeyEventArgs>(currentscene_KeyDown);
        }

        void currentscene_KeyDown(object sender, KeyEventArgs e)
        {
            if (currentscene != null)
            {
                switch (e.KeyCode)
                {
                    case Keys.Enter:
                        InteractWithObjectAhead();
                        break;
                }
            }
        }

        void currentscene_DXKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (currentscene != null)
            {
                switch (e.KeyCode)
                {
                    case Keys.W:
                    case Keys.Up:
                        currentscene.Move(Direction.Up);
                        break;
                    case Keys.S:
                    case Keys.Down:
                        currentscene.Move(Direction.Down);
                        break;
                    case Keys.A:
                    case Keys.Left:
                        currentscene.Move(Direction.Left);
                        break;
                    case Keys.D:
                    case Keys.Right:
                        currentscene.Move(Direction.Right);
                        break;
                }
            }
        }

        void InteractWithObjectAhead()
        {
            if (player == null) return;
            int ox = -1, oy = -1;
            switch (player.Direction)
            {
                case Direction.Up:
                    ox = m.CameraX;
                    oy = m.CameraY - 1;
                    break;
                case Direction.Down:
                    ox = m.CameraX;
                    oy = m.CameraY + 1;
                    break;
                case Direction.Left:
                    ox = m.CameraX - 1;
                    oy = m.CameraY;
                    break;
                case Direction.Right:
                    ox = m.CameraX + 1;
                    oy = m.CameraY;
                    break;
            }
            // avoid looking off the map. bad bad bad
            if (ox < 0 || oy < 0 || ox >= m.Width || oy >= m.Height)
            {
                return;
            }
            MapObject mo = m.GetCharacter(ox, oy);
            if (mo != null)
            {
                if (mo is ICharacter)
                {
                    if ((mo as ICharacter).Use != null)
                    {
                        (mo as ICharacter).Use(this);
                    }
                }
            }
        }


        // The story needs a state. So here it is
        public Map m;
        public ICharacter player;
        public PlayScene currentscene;
        public NarrationDialog narrationdialog;
        public MenuDialog menudialog;
        public StaticImage staticImage;
    }

    public class ObjectsAssembly
    {
        CodeLoader cl = new CodeLoader();
        StringBuilder sb = new StringBuilder();
        TileBank gw = new TileBank();

        public ObjectsAssembly(TileBank gw)
        {
            sb.Append("using System;\n");
            sb.Append("using EmeraldDream;\n");
            sb.Append("using EmeraldLibrary;\n");
            sb.Append("using System.Windows.Forms;\n");
            sb.Append("using System.Collections.Generic;\n");

            sb.Append("namespace EmeraldDream\n");
            sb.Append("{\n");

            this.gw = gw;
        }

        List<string> floorlist = new List<string>();
        List<string> objectlist = new List<string>();

        public void AddFloor(string floorname, string filename){

            floorlist.Add(floorname);

            bool passable = true;
            int tileindex = 0;

            using (StreamReader sr = new StreamReader(filename))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (line.StartsWith("tile"))
                    {
                        string tilename = line.Split(':')[1];
                        tileindex = this.gw.GetTileByName(tilename);
                    }
                    if (line.StartsWith("passable"))
                    {
                        passable = bool.Parse(line.Split(':')[1]);
                    }
                }
            }

            sb.Append("    public class " + "floor_" + floorname + " : MapFloor \n");
            sb.Append("    {\n");

            sb.Append("        string mode = \"stand\";\n");
            sb.Append("        int frame;\n");

            sb.Append("        Dictionary<string, int[,]> tileSets = new Dictionary<string, int[,]>();\n");

            sb.Append("        public void SetMode(string mode) { this.mode = mode; } \n");
            sb.Append("        public void SetFrameNumber(int num) { this.frame = num; } \n");

            // TODO use tileSets and add animation!

            sb.Append("        public bool Passable\n");
            sb.Append("        {\n");
            sb.Append("            get { return " + passable.ToString().ToLower() + "; }\n");
            sb.Append("        }\n");

            sb.Append("        public int TileIndex\n");
            sb.Append("        {\n");
            sb.Append("            get { return " + tileindex.ToString() + "; }\n");
            sb.Append("        }\n");

            sb.Append("    }\n");
        }

        public void AddObject(string objectname, string filename)
        {
            objectlist.Add(objectname);

            int upindex = 0;
            int downindex = 0;
            int leftindex = 0;
            int rightindex = 0;



            Func<string, int[]> HandleTileName = x =>
            {

                string name = x.Split('?')[0];

                int[] result = new int[4];

                int tileindex = this.gw.GetTileByName(name);
                result[0] = tileindex;
                result[1] = tileindex;
                result[2] = tileindex;
                result[3] = tileindex;
                
                if (x.Split('?').Length == 2)
                {
                    string coords = x.Split('?')[1];
                    string[] coordset = coords.Split('&');
                    if (coordset.Length == 4)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            string[] coordindivs = coordset[i].Split(',');
                            tileindex = this.gw.GetTileByName(
                                name,
                                new Rectangle(
                                int.Parse(coordindivs[0]),
                                int.Parse(coordindivs[1]),
                                int.Parse(coordindivs[2]),
                                int.Parse(coordindivs[3])));

                            result[i] = tileindex;
                        }

                    }
                    else
                    {
                        string[] coordindivs = coordset[0].Split(',');
                        tileindex = this.gw.GetTileByName(
                            name,
                            new Rectangle(
                            int.Parse(coordindivs[0]),
                            int.Parse(coordindivs[1]),
                            int.Parse(coordindivs[2]),
                            int.Parse(coordindivs[3])));

                        result[0] = tileindex;
                        result[1] = tileindex;
                        result[2] = tileindex;
                        result[3] = tileindex;
                    }
                }

                return result;
            };

            Dictionary<string, int[,]> tileSets = new Dictionary<string, int[,]>();

            string mode = "stand";

            using (StreamReader sr = new StreamReader(filename))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();

                    if (line.StartsWith("-"))
                    {
                        mode = line.Substring(1);
                        tileSets[mode] = new int[4, 4];
                    }
                    else if (line.StartsWith("up"))
                    {
                        string tilename = line.Split(':')[1];
                        int[] tiles = HandleTileName(tilename);
                        tileSets[mode][(int)Direction.Up, 0] = tiles[0];
                        tileSets[mode][(int)Direction.Up, 1] = tiles[1];
                        tileSets[mode][(int)Direction.Up, 2] = tiles[2];
                        tileSets[mode][(int)Direction.Up, 3] = tiles[3];
                    }
                    else if (line.StartsWith("down"))
                    {
                        string tilename = line.Split(':')[1];
                        int[] tiles = HandleTileName(tilename);
                        tileSets[mode][(int)Direction.Down, 0] = tiles[0];
                        tileSets[mode][(int)Direction.Down, 1] = tiles[1];
                        tileSets[mode][(int)Direction.Down, 2] = tiles[2];
                        tileSets[mode][(int)Direction.Down, 3] = tiles[3];
                    }
                    else if (line.StartsWith("left"))
                    {
                        string tilename = line.Split(':')[1];
                        int[] tiles = HandleTileName(tilename);
                        tileSets[mode][(int)Direction.Left, 0] = tiles[0];
                        tileSets[mode][(int)Direction.Left, 1] = tiles[1];
                        tileSets[mode][(int)Direction.Left, 2] = tiles[2];
                        tileSets[mode][(int)Direction.Left, 3] = tiles[3];
                    }
                    else if (line.StartsWith("right"))
                    {
                        string tilename = line.Split(':')[1];
                        int[] tiles = HandleTileName(tilename);
                        tileSets[mode][(int)Direction.Right, 0] = tiles[0];
                        tileSets[mode][(int)Direction.Right, 1] = tiles[1];
                        tileSets[mode][(int)Direction.Right, 2] = tiles[2];
                        tileSets[mode][(int)Direction.Right, 3] = tiles[3];
                    }
                }
            }

            sb.Append("    public class " + "object_" + objectname + " : MapObject, ICharacter \n");
            sb.Append("    {\n");

            sb.Append("        Action<Story> onUse = null;\n");

            sb.Append("        string mode = \"stand\";\n");
            sb.Append("        int frame;\n");

            sb.Append("        Dictionary<string, int[,]> tileSets = new Dictionary<string, int[,]>();\n");

            sb.Append("        public void SetMode(string mode) { this.mode = mode; } \n");
            sb.Append("        public void SetFrameNumber(int num) { this.frame = num; } \n");

            sb.Append("        public object_" + objectname + "() { \n");

            foreach (var kvpair in tileSets)
            {
                sb.Append("            tileSets[\"" + kvpair.Key + "\"] = new int[4,4];\n");
                for (int x = 0; x < 4; x++)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        sb.Append("            tileSets[\"" + kvpair.Key + "\"][" + x + "," + y + "] = " + kvpair.Value[x, y] + ";\n");
                    }
                }
            }

            sb.Append("        }\n");

            sb.Append("        Direction dir = Direction.Up;\n");

            sb.Append("        public Direction Direction\n");
            sb.Append("        {\n");
            sb.Append("            get { return dir; }\n");
            sb.Append("            set { dir = value; }\n");
            sb.Append("        }\n");

            sb.Append("        public int TileIndex\n");
            sb.Append("        {\n");
            sb.Append("            get { return tileSets[mode][(int)dir,frame]; }\n");
            sb.Append("        }\n");

            sb.Append("        public Action<Story> Use\n");
            sb.Append("        {\n");
            sb.Append("            get { return onUse; }\n");
            sb.Append("            set { onUse = value; }\n");
            sb.Append("        }\n");


            sb.Append("    }\n");
        }

        Assembly objectsassembly = null;

        public void Compile()
        {
            sb.Append("}\n");

            objectsassembly = cl.Compile("ObjectsAssembly", sb.ToString());
        }

        public Dictionary<string, Type> GetFloorTypes()
        {
            Dictionary<string, Type> floorNameToTypeMap = new Dictionary<string, Type>();

            floorlist.ForEach(floor => floorNameToTypeMap[floor] = objectsassembly.GetType("EmeraldDream." + "floor_" + floor));

            return floorNameToTypeMap;
        }

        public Dictionary<string, Type> GetObjectTypes()
        {
            Dictionary<string, Type> objectNameToTypeMap = new Dictionary<string, Type>();

            objectlist.ForEach(floor => objectNameToTypeMap[floor] = objectsassembly.GetType("EmeraldDream." + "object_" + floor));

            return objectNameToTypeMap;
        }
    }
}
