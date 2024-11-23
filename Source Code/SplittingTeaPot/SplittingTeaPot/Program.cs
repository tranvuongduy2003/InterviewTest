using System.Numerics;

namespace SplittingTeaPot
{
    public class Vertex
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vertex(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }


    public class Face
    {
        public List<int> VertexIndices { get; set; }

        public Face()
        {
            VertexIndices = new List<int>();
        }
    }

    public class ObjSplitter
    {
        private List<Vertex> vertices;
        private List<Face> faces;
        private Vector3 center;

        public ObjSplitter()
        {
            vertices = new List<Vertex>();
            faces = new List<Face>();
        }

        public void ReadFileObj(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts[0] == "v")
                {
                    float x = float.Parse(parts[1]);
                    float y = float.Parse(parts[2]);
                    float z = float.Parse(parts[3]);
                    vertices.Add(new Vertex(x, y, z));
                }
                else if (parts[0] == "f")
                {
                    Face face = new Face();
                    for (int i = 1; i < parts.Length; i++)
                    {
                        // Handle both formats: f v1 v2 v3 or f v1/vt1/vn1 v2/vt2/vn2 v3/vt3/vn3
                        int vertexIndex = int.Parse(parts[i].Split('/')[0]) - 1;
                        face.VertexIndices.Add(vertexIndex);
                    }
                    faces.Add(face);
                }
            }

            FindTheCenter();
        }

        private void FindTheCenter()
        {
            if (vertices.Count == 0) return;

            float minX = vertices.Min(v => v.X);
            float maxX = vertices.Max(v => v.X);
            float minY = vertices.Min(v => v.Y);
            float maxY = vertices.Max(v => v.Y);
            float minZ = vertices.Min(v => v.Z);
            float maxZ = vertices.Max(v => v.Z);

            center = new Vector3(
                (minX + maxX) / 2,
                (minY + maxY) / 2,
                (minZ + maxZ) / 2
            );
        }

        public void SplitObject(string exportFilePrefix = "file")
        {
            for (int part = 0; part < 4; part++)
            {
                List<Vertex> verticesPart = new List<Vertex>();
                List<Face> facesPart = new List<Face>();
                Dictionary<int, int> vertexConversionTable = new Dictionary<int, int>();

                // Identify condition for each part
                Func<Vertex, bool> findCondition = part switch
                {
                    0 => v => v.X <= center.X && v.Y >= center.Y, // Top-left
                    1 => v => v.X > center.X && v.Y >= center.Y,  // Top-right
                    2 => v => v.X <= center.X && v.Y < center.Y,  // Bottom-left
                    3 => v => v.X > center.X && v.Y < center.Y,   // Bottom-right
                    _ => throw new ArgumentException("Invalid part")
                };

                // Filter which vertices belong to part
                for (int i = 0; i < vertices.Count; i++)
                {
                    if (findCondition(vertices[i]))
                    {
                        vertexConversionTable[i] = verticesPart.Count;
                        verticesPart.Add(vertices[i]);
                    }
                }

                // Filter faces whose vertices all belong to this part
                foreach (Face face in faces)
                {
                    if (face.VertexIndices.All(vi => vertexConversionTable.ContainsKey(vi)))
                    {
                        Face newFace = new Face();
                        foreach (int vi in face.VertexIndices)
                        {
                            newFace.VertexIndices.Add(vertexConversionTable[vi]);
                        }
                        facesPart.Add(newFace);
                    }
                }

                // Export file
                ExportObjFile($"results/{exportFilePrefix}{part + 1}.obj", verticesPart, facesPart);
            }
        }

        private void ExportObjFile(string fileName, List<Vertex> vertices, List<Face> faces)
        {
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                // Write veritces
                foreach (Vertex v in vertices)
                {
                    writer.WriteLine($"v {v.X} {v.Y} {v.Z}");
                }

                // Write faces (index starts from 1 in .obj file)
                foreach (Face f in faces)
                {
                    writer.WriteLine($"f {string.Join(" ", f.VertexIndices.Select(i => (i + 1).ToString()))}");
                }
            }
        }
    }
    public class Program
    {
        public static void Main()
        {
            ObjSplitter splitter = new ObjSplitter();
            splitter.ReadFileObj("teapot.obj");
            Console.WriteLine("Read file teapot.obj successfully!");
            splitter.SplitObject();
            Console.WriteLine("Processed!");
            Console.ReadLine();
        }
    }
}
