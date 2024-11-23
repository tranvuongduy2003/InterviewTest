using System.Xml;

namespace TreeViewGenerator
{
    public class XNode : TreeNode
    {
        public string NodeId { get; set; }
        public string NodeText { get; set; }

        public XNode(string id, string text) : base(text)
        {
            NodeId = id;
            NodeText = text;
        }
    }

    public partial class UploadForm : Form
    {
        private TreeView treeView;
        private Button btnLoad;

        public UploadForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Size = new System.Drawing.Size(800, 600);

            treeView = new TreeView
            {
                Dock = DockStyle.Fill
            };

            btnLoad = new Button
            {
                Text = "Load XML",
                Dock = DockStyle.Top,
                Size = new System.Drawing.Size(800, 40)
            };
            btnLoad.Click += BtnLoad_Click;

            this.Controls.Add(treeView);
            this.Controls.Add(btnLoad);
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    LoadXmlFile(openFileDialog.FileName);
                }
            }
        }

        private void LoadXmlFile(string filename)
        {
            try
            {
                treeView.Nodes.Clear();
                XmlDocument doc = new XmlDocument();
                doc.Load(filename);

                ProcessXmlNode(doc.DocumentElement, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading XML: {ex.Message}");
            }
        }

        private void ProcessXmlNode(XmlNode xmlNode, TreeNode parentNode)
        {
            if (xmlNode == null) return;

            string nodeId = xmlNode.Attributes?["id"]?.Value ?? "";
            string nodeText = xmlNode.Attributes?["text"]?.Value ?? xmlNode.Name;

            XNode newNode = new XNode(nodeId, nodeText);

            if (parentNode == null)
                treeView.Nodes.Add(newNode);
            else
                parentNode.Nodes.Add(newNode);

            foreach (XmlNode childNode in xmlNode.ChildNodes)
            {
                if (childNode.NodeType == XmlNodeType.Element)
                {
                    ProcessXmlNode(childNode, newNode);
                }
            }
        }
    }
}
