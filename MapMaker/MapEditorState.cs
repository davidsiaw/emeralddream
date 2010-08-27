using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using DockDotNET;
using System.Windows.Forms;
using EmeraldLibrary;
using EmeraldDream;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;

namespace MapMaker
{
    public class MapEditorState
    {
        struct ToolWindowInfo
        {
            public DockWindow dw;
            public ToolStripMenuItem tsmi;
        }

        struct MapEditorInfo
        {
            public string mapname;
            public MapEditor map;
        }

        public struct MapInfo
        {
            public string fulldir;
            public bool changed;
            public Map map;
        }

        public DockManager dm;
        public ToolStripStatusLabel coordlabel;
        public string resDir = "res";
        public Dictionary<string, MapInfo> maps = new Dictionary<string, MapInfo>();
        public Type selectedFloor = null;
        public TileBank tilebank = new TileBank();

        Dictionary<string, ToolWindowInfo> toolwindows = new Dictionary<string, ToolWindowInfo>();
        Dictionary<string, MapEditorInfo> openMaps = new Dictionary<string, MapEditorInfo>();

        public Dictionary<string, Type> floortypes = new Dictionary<string, Type>();
        public Dictionary<string, Type> objecttypes = new Dictionary<string, Type>();
        ObjectsAssembly fa;

        public void SaveMaps()
        {
            maps.ToList().ForEach(x =>
            {
                if (x.Value.changed)
                {
                    string dir = x.Value.fulldir;
                }
            });
        }

        public void ApplyFloor(Map map, int x, int y)
        {
            map.SetFloor((MapFloor)Activator.CreateInstance(selectedFloor), x, y);
        }


        public void RegisterWindow(string name, DockWindow window, ToolStripMenuItem menuitem)
        {
            ToolWindowInfo twi = new ToolWindowInfo();
            twi.dw = window;
            twi.tsmi = menuitem;
            menuitem.Checked = true;
            toolwindows[name] = twi;
        }

        public void DeregisterWindow(string name)
        {
            if (!IsWindowRegistered(name)) { return; }
            toolwindows[name].tsmi.Checked = false;
            toolwindows.Remove(name);
        }

        Dictionary<string, Type> toolwindowtypes = new Dictionary<string, Type>();

        public void ToggleWindow(string name, ToolStripMenuItem tsmi)
        {
            if (!IsWindowRegistered(name))
            {
                OpenWindow(name, tsmi);
            }
            else
            {
                CloseWindow(name);
            }
        }

        private void OpenWindow(string name, ToolStripMenuItem tsmi)
        {

            object f = Activator.CreateInstance(toolwindowtypes[name], this, tsmi);
            dm.DockWindow((DockWindow)f, DockStyle.Right);
            (f as DockWindow).Show();
        }

        public void CloseWindow(string name)
        {
            if (!IsWindowRegistered(name)) { return; }
            toolwindows[name].dw.Close();
        }

        public bool IsWindowRegistered(string name)
        {
            return toolwindows.ContainsKey(name);
        }

        public void Init()
        {
            fa = new ObjectsAssembly(tilebank);

            // Grab tiles
            string[] files = Directory.GetFiles(Path.Combine(resDir, "res/tiles"), "*.png");
            files.ToList().ForEach(x =>
            {
                string tilename = Path.GetFileNameWithoutExtension(x);
                tilebank.LoadTile(tilename, x);
            });

            // Grab floor types
            string[] floors = Directory.GetFiles(Path.Combine(resDir, "res/floors"), "*.floor");
            floors.ToList().ForEach(x =>
            {
                string floorname = Path.GetFileNameWithoutExtension(x);
                fa.AddFloor(floorname, x);
            });

            // Grab object types
            string[] objects = Directory.GetFiles(Path.Combine(resDir, "res/objects"), "*.desc");
            objects.ToList().ForEach(x =>
            {
                string objname = Path.GetFileNameWithoutExtension(x);
                fa.AddObject(objname, x);
            });
            // Compile the types and prepare them
            fa.Compile();

            floortypes = fa.GetFloorTypes();
            objecttypes = fa.GetObjectTypes();

            string[] mapdirs = Directory.GetDirectories(Path.Combine(resDir, "maps"));
            mapdirs.ToList().ForEach(x =>
            {
                MapInfo mi = new MapInfo();
                mi.fulldir = x;
                mi.changed = false;
                mi.map = null;
                maps[Path.GetFileNameWithoutExtension(x)] = mi;
                //Map m = LoadMap(tilebank, x);
                //string mapname = Path.GetFileNameWithoutExtension(x);
                //this.maps[mapname] = m;
            });

            toolwindowtypes["MapList"] = typeof(MapList);
            toolwindowtypes["Floors"] = typeof(Floors);

            selectedFloor = floortypes.First().Value;
        }

        public void OpenMapEditor (string map) {
            if (!IsMapEditorRegistered(map))
            {
                MapEditor editor = new MapEditor(tilebank, this, map);
                MapEditorInfo mi = new MapEditorInfo();
                mi.mapname = map;
                mi.map = editor;
                openMaps[map] = mi;
                Map m = LoadMap(editor, editor.pictureBox1, map);
                editor.pictureBox1.Scene = new Scene(m);
                dm.DockWindow(editor, DockStyle.Fill);
                editor.Text = "map: " + map;
                editor.Show();
                editor.Map = m;
                editor.SceneHasBeenSet();
            }
        }

        public void CloseMapEditor(string map)
        {
            if (IsMapEditorRegistered(map))
            {
                openMaps[map].map.pictureBox1.Scene.Stop();
                openMaps[map].map.Close();
                openMaps.Remove(map);
            }
        }

        public bool IsMapEditorRegistered(string map)
        {
            return openMaps.ContainsKey(map);
        }

        Map LoadMap(MapEditor editor, GameControl gc, string map)
        {
            if (maps[map].map != null)
            {
                maps[map].map.GameControl = gc;
                return maps[map].map;
            }


            string mapdir = maps[map].fulldir;

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
                    m = new Map(gc, w, h);

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
                        }

                        match = objectnamelabel.Match(line);
                        if (match.Success) break;
                    }
                }

            }

            return m;
        }
       
    }
}
