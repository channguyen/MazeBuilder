using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace WallMaze {
    public class MazeInfoDialog : Form {
        private Label widthLabel;
        private TextBox widthTextBox;

        private Label heightLabel;
        private TextBox heightTextBox;

        private Label spacingLabel;
        private TextBox spacingTextBox;

        private Button okButton;

        private int width;
        private int height;
        private int spacing;
    
        public MazeInfoDialog() {
            InitializeComponent();
        }

        private void InitializeComponent() {
            this.heightTextBox = new System.Windows.Forms.TextBox();
            this.widthLabel = new System.Windows.Forms.Label();
            this.heightLabel = new System.Windows.Forms.Label();
            this.widthTextBox = new System.Windows.Forms.TextBox();
            this.spacingLabel = new System.Windows.Forms.Label();
            this.spacingTextBox = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.SuspendLayout();

            this.heightTextBox.Location = new System.Drawing.Point(82, 18);
            this.heightTextBox.Size = new System.Drawing.Size(100, 20);
            this.heightTextBox.TabIndex = 0;

            this.widthLabel.AutoSize = true;
            this.widthLabel.Location = new System.Drawing.Point(25, 21);
            this.widthLabel.Size = new System.Drawing.Size(35, 13);
            this.widthLabel.TabIndex = 1;
            this.widthLabel.Text = "Width: ";

            this.heightLabel.AutoSize = true;
            this.heightLabel.Location = new System.Drawing.Point(25, 50);
            this.heightLabel.Size = new System.Drawing.Size(35, 13);
            this.heightLabel.TabIndex = 2;
            this.heightLabel.Text = "Height: ";

            this.widthTextBox.Location = new System.Drawing.Point(82, 47);
            this.widthTextBox.Size = new System.Drawing.Size(100, 20);
            this.widthTextBox.TabIndex = 3;

            this.spacingLabel.AutoSize = true;
            this.spacingLabel.Location = new System.Drawing.Point(25, 80);
            this.spacingLabel.Size = new System.Drawing.Size(35, 13);
            this.spacingLabel.TabIndex = 4;
            this.spacingLabel.Text = "Spacing: ";
          
            this.spacingTextBox.Location = new System.Drawing.Point(82, 73);
            this.spacingTextBox.Size = new System.Drawing.Size(100, 20);
            this.spacingTextBox.TabIndex = 5;
           
            this.okButton.Location = new System.Drawing.Point(107, 104);
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 6;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += delegate(object sender, EventArgs args) {
                try {
                    width = Int32.Parse(widthTextBox.Text.ToString());
                    height = Int32.Parse(heightTextBox.Text.ToString());
                    spacing = Int32.Parse(spacingTextBox.Text.ToString());
                }
                catch (Exception e) {
                    MessageBox.Show("Invalid format!");
                    this.DialogResult = DialogResult.Cancel;
                    return;
                }
                
                this.DialogResult = DialogResult.OK;
            };

            this.ClientSize = new System.Drawing.Size(223, 139);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.spacingTextBox);
            this.Controls.Add(this.spacingLabel);
            this.Controls.Add(this.widthTextBox);
            this.Controls.Add(this.heightLabel);
            this.Controls.Add(this.widthLabel);
            this.Controls.Add(this.heightTextBox);
            this.Name = "MazeInfoDialog";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        public int Width {
            get {
                return width; 
            }
        }

        public int Height {
            get {
                return height;
            }
        }

        public int Spacing {
            get {
                return spacing;
            }
        }
    }
}
