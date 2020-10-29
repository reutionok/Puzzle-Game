using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Puzzle
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        OpenFileDialog openFileDialog = null;
        Image image;
        PictureBox picBoxWhole = null;

        private void buttonImageBrowse_Click(object sender, EventArgs e)
        {
            if (openFileDialog == null)
                openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textboxImagePath.Text = openFileDialog.FileName;
                image = CreateBitmapImage(Image.FromFile(openFileDialog.FileName));
                if (picBoxWhole == null)
                {
                    picBoxWhole = new PictureBox();
                    picBoxWhole.Height = groupboxPuzzle.Height;
                    picBoxWhole.Width = groupboxPuzzle.Width;
                    groupboxPuzzle.Controls.Add(picBoxWhole);
                }
                picBoxWhole.Image = image;
            }
        }
        private Bitmap CreateBitmapImage(Image image)
        {
            Bitmap objBmpImage = new Bitmap(groupboxPuzzle.Width, groupboxPuzzle.Height);
            Graphics objGraphics = Graphics.FromImage(objBmpImage);
            objGraphics.Clear(Color.White);

            objGraphics.DrawImage(image,
                new Rectangle(0, 0, groupboxPuzzle.Width, groupboxPuzzle.Height));                
            objGraphics.Flush();

            return objBmpImage;
        }
        private void CreateBitmapImage(Image image, Image [] images, int index, int numRow, int numCol, int unitX, int unitY) 
        {
            if (image != null)
            {
                images[index] = new Bitmap(unitX, unitY);
                Graphics objGraphics = Graphics.FromImage(images[index]);
                objGraphics.Clear(Color.White);

                objGraphics.DrawImage(image,
                    new Rectangle(0, 0, unitX, unitY),
                    new Rectangle(unitX * (index % numCol), unitY * (index / numCol), unitX, unitY),
                    GraphicsUnit.Pixel);
                objGraphics.Flush();
            }
            else
                labelStatus.Text = "Choose image please!";
        }

        private void Shuffle(ref int[] array)
        {
            Random rng = new Random();
            int n = array.Length;
            while (n > 1)
            {
                int k = rng.Next(n);
                n--;
                int temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }

        PictureBox[] picBoxes=null;
        Image[] images = null;
        const int level_1_num = 4;
        const int level_2_num = 9;
        const int level_3_num = 16;
        int currentLevel = 0;

        private void buttonLevel1_Click(object sender, EventArgs e)
        {
            currentLevel = level_1_num;
            labelStatus.Text = "Level 1 is ongoing ...";
            PlayLevel();

        }
        private void buttonLevel2_Click(object sender, EventArgs e)
        {
            currentLevel = level_2_num;
            labelStatus.Text = "Level 2 is ongoing ...";
            PlayLevel();
        }

        private void buttonLevel3_Click(object sender, EventArgs e)
        {
            currentLevel = level_3_num;
            labelStatus.Text = "Level 3 is ongoing ...";
            PlayLevel();
        }

        private void PlayLevel()
        {
            if (picBoxWhole != null)
            {
                groupboxPuzzle.Controls.Remove(picBoxWhole);
                picBoxWhole.Dispose();
                picBoxWhole = null;
            }

            if (picBoxes == null || picBoxes.Length<currentLevel)
            {
                images = new Image[currentLevel];
                picBoxes = new PictureBox[currentLevel];
                groupboxPuzzle.Controls.Clear();
            }

            int numRow = (int)Math.Sqrt(currentLevel);
            int numCol = numRow;
            int unitX = groupboxPuzzle.Width / numRow;
            int unitY = groupboxPuzzle.Height / numCol;
            int[] indice = new int[currentLevel];
            for (int i = 0; i < currentLevel; i++)
            {
                indice[i] = i;
                if (picBoxes[i] == null)
                {
                    picBoxes[i] = new MyPictureBox();
                    picBoxes[i].Click += new EventHandler(OnPuzzleClick);
                    picBoxes[i].BorderStyle = BorderStyle.Fixed3D;
                }
                picBoxes[i].Width = unitX;
                picBoxes[i].Height = unitY;

                ((MyPictureBox)picBoxes[i]).Index = i;

                CreateBitmapImage(image, images, i, numRow, numCol, unitX, unitY);

                picBoxes[i].Location = new Point(unitX * (i % numCol), unitY * (i / numCol));
                if (!groupboxPuzzle.Controls.Contains(picBoxes[i]))
                    groupboxPuzzle.Controls.Add(picBoxes[i]);
            }

            Shuffle(ref indice);
            for (int i = 0; i < currentLevel; i++)
            {
                picBoxes[i].Image = images[indice[i]];
                ((MyPictureBox)picBoxes[i]).ImageIndex = indice[i];
            }
            
        }


        MyPictureBox firstBox = null;
        MyPictureBox secondBox = null;
        public void OnPuzzleClick(object sender, EventArgs e)
        {
            if (firstBox == null)
            {
                firstBox = (MyPictureBox)sender;
                firstBox.BorderStyle = BorderStyle.FixedSingle;
            }
            else if (secondBox == null)
            {
                secondBox = (MyPictureBox)sender;
                firstBox.BorderStyle = BorderStyle.Fixed3D;
                secondBox.BorderStyle = BorderStyle.FixedSingle;
                SwitchImages(firstBox, secondBox);
                firstBox = null;
                secondBox = null;
            }
            //else
            //{
            //    firstBox = secondBox;
            //    firstBox.BorderStyle = BorderStyle.Fixed3D;
            //    secondBox = (MyPictureBox)sender;
            //    secondBox.BorderStyle = BorderStyle.FixedSingle;
            //    SwitchImages(firstBox, secondBox);
            //}
        }
        private void SwitchImages(MyPictureBox box1, MyPictureBox box2)
        {
            int tmp = box2.ImageIndex;
            box2.Image = images[box1.ImageIndex];
            box2.ImageIndex = box1.ImageIndex;
            box1.Image = images[tmp];
            box1.ImageIndex = tmp;
            if (isSeccessful())
            {
                labelStatus.Text = "Well Done!!!";
            }
        }

        private bool isSeccessful()
        {
            for (int i = 0; i < currentLevel; i++)
            {
                if (((MyPictureBox)picBoxes[i]).ImageIndex != ((MyPictureBox)picBoxes[i]).Index)
                    return false;
            }
            return true;
        }

        private void buttonSolution_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < currentLevel; i++)
            {
                picBoxes[i].Image = images[i];
                ((MyPictureBox)picBoxes[i]).ImageIndex = i;
                labelStatus.Text = "Well Done!!!";
            }
        }
    }

}    

