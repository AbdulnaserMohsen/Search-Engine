using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using TikaOnDotNet.TextExtraction;

using System.IO;

namespace search_Engind_DDB_
{

    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            magic();
            allwords();
            makepostion();
            countFreq();
        }
        

        TextExtractor tikaExtract = new TextExtractor(); //tika
        string[] files = Directory.GetFiles(Application.StartupPath + @"\word files\"); //all files
        List<string> content = new List<string>(); // content of files
        List<DistinctWords> distinctwords = new List<DistinctWords>(); //all words distinct(class) have word and its frequency
        List<positions> postion = new List<positions>(); // (class) has each word and its document id and postion in the file and if has sikp point or not and the skip point
        
        void  magic()//get content from all files and save it in content list
        {
            for (int i = 0; i < files.Length; i++)
            {
                content.Add(tikaExtract.Extract(files[i]).Text.Trim());
            }

        }


        void allwords()//get words from content list and clean it and delete repeated words the put the new words in the distinct words list
        {
            List<string> post = new List<string>();
            for (int i = 0; i < content.Count; i++)
            {
                post = content[i].Split().Distinct().ToList();
                post = clean(post);
                post = post.Distinct().ToList();
                deleteRepeat(post);
                post = post.Distinct().ToList();
                for (int j = 0; j < post.Count; j++)
                {
                    DistinctWords d = new DistinctWords();
                    d.distictword = post[j];
                    distinctwords.Add(d);

                }

            }

        }


        List<string> clean(List<string> words) //remove spaces and special characters from the beginiing and end of word and  convert words to lower case
        {
            words.RemoveAll(item => item == "");
            for (int i=0; i<words.Count; i++)
            {
                words[i] = words[i].ToLower();
                words[i] = words[i].Trim();
                words[i] = words[i].Trim('.');
                words[i] = words[i].Trim(',');
                words[i] = words[i].Trim(':');
                words[i] = words[i].Trim('!');
                words[i] = words[i].Trim('@');
                words[i] = words[i].Trim('$');
                words[i] = words[i].Trim('%');
                words[i] = words[i].Trim('^');
                words[i] = words[i].Trim('&');
                words[i] = words[i].Trim('*');
                words[i] = words[i].Trim('(');
                words[i] = words[i].Trim(')');
                words[i] = words[i].Trim('-');
                words[i] = words[i].Trim('_');
                words[i] = words[i].Trim('+');
                words[i] = words[i].Trim('=');
                words[i] = words[i].Trim(';');
                words[i] = words[i].Trim('\'');
                words[i] = words[i].Trim('|');
                words[i] = words[i].Trim('"');
                words[i] = words[i].Trim('?');
                words[i] = words[i].Trim('/');
                words[i] = words[i].Trim('<');
                words[i] = words[i].Trim('>');
                words[i] = words[i].Trim('[');
                words[i] = words[i].Trim(']');
                words[i] = words[i].Trim('{');
                words[i] = words[i].Trim('}');
                words[i] = words[i].Trim('~');
                words[i] = words[i].Trim('`');
                words[i] = words[i].Trim('⌂');
                words[i] = words[i].Trim('£');
                words[i] = words[i].Trim('®');
                words[i] = words[i].Trim('©');
                words[i] = words[i].Trim(' ');
                words[i] = words[i].Trim('●');
                words[i] = words[i].Trim('–');
                words[i] = words[i].Trim('€');
                words[i] = words[i].Trim(' ');
                //words[i] = words[i].Replace("=", " = ");
            }
            words.RemoveAll(item => item == "");
            return words;
        }


        void deleteRepeat(List<string> words) //delete words that is in the distinct words list
        {
            List<string> removeIndexs = new List<string>();
            for (int i = 0; i < words.Count; i++)
            {
                for (int j = 0; j < distinctwords.Count; j++)
                {
                    if (words[i] == distinctwords[j].distictword)
                    {
                        //words.Remove(words[i]);
                        removeIndexs.Add(words[i]);
                        break;
                    }
                }
            }
            for (int i=0; i<removeIndexs.Count; i++)
            {
                words.Remove(removeIndexs[i]);
            }
        }

        
        


        void makepostion()//put every word in the postion list with its document id and its postion
        {
            List<string> words = new List<string>();
            
            for (int i = 0; i < files.Length; i++)
            {
                words = content[i].Split().ToList();
                for (int j = 0; j< words.Count; j++)
                {
                    words = clean(words);
                    for (int d = 0; d < distinctwords.Count; d++)
                    {
                        if (words[j] == distinctwords[d].distictword)
                        {
                            //Path.GetFileName(files[i])
                            positions p = new positions(i,distinctwords[d].distictword, j);
                            postion.Add(p);
                            break;
                        }
                    }
                }
                words.Clear();
            }

        }

        void countFreq()//count every distinct word repeated in how many document
        {
            //get frequencey of the term

            for (int i = 0; i < distinctwords.Count; i++)
            {
                int frequec = 0;
                for (int j = 0; j < files.Length; j++)
                {
                    for (int p = 0; p < postion.Count; p++)
                    {
                        if (distinctwords[i].distictword == postion[p].term   &&   j == postion[p].doc_id)
                        {
                            frequec++;
                            break;
                        }
                    }
                }
                distinctwords[i].freq = frequec;
            }
            
        }

        void makeSkipPoints()//make skip points to the each term has sqrt(frequencey) more than 2(not completed yet)
        {
            for (int i=0;  i<distinctwords.Count; i++)
            {
                if (Math.Sqrt(distinctwords[i].freq) > 2)
                {
                    for (int j = 0; j < postion.Count; j++)
                    {
                        if (distinctwords[i].distictword == postion[j].term)
                        {
                            postion[j].haveSkip = true;
                            postion[j].skipPoint = 1;
                        }
                    }
                }
            }

        }



        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            richTextBox1.Text = Path.GetFileName(files[0])+"\n";
            richTextBox1.Text += content[0];

            richTextBox2.Clear();
            for (int i=0; i<distinctwords.Count; i++)
            {
                richTextBox2.Text += distinctwords[i].distictword+"  : "+distinctwords[i].freq+"\n";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox2.Clear();
            for (int i = 0; i < postion.Count; i++)
            {
               if (postion[i].term == "university")
                {
                    richTextBox2.Text += postion[i].term + "  : " + postion[i].doc_id + " : " + postion[i].position + "\n";
                }
            }

            
            richTextBox1.Clear();
            richTextBox1.Text = Path.GetFileName(files[1]) + "\n";
            richTextBox1.Text +=  content[1];
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if ("e-commerce" == "​e-commerce")
            {
                richTextBox1.Clear();
                richTextBox1.Text = "ده باينلوا مرار طافح";
            }
            else
            {
                richTextBox1.Clear();
                richTextBox1.Text = "ماشي تمام";
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
        }

        private void button6_Click(object sender, EventArgs e)
        {
            richTextBox2.Clear();
            StringBuilder myStringBuilder = new StringBuilder();
            foreach (var p in postion)
            {
                myStringBuilder.AppendLine(p.term+"  : "+ p.doc_id +" : "+p.position+"\n");//if we put it in rich box directly it will be so slow
            }
            richTextBox2.Text = myStringBuilder.ToString();
            
        }

    }



    class DistinctWords
    {
        public string distictword;
        public int freq = 0;
    }


    class positions
    {

        public int doc_id;
        public string term;
        public int position;
        public bool haveSkip = false;
        public int skipPoint;

        public positions(int Document_id, string Term, int Position)
        {
            doc_id = Document_id;
            term = Term;
            position = Position;
        }

    }

}
