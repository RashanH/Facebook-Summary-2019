using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Facebook_Summary_2019
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static String[] GetFilesFrom(String searchFolder, String[] filters, bool isRecursive)
        {
            List<String> filesFound = new List<String>();
            var searchOption = isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            foreach (var filter in filters)
            {
                filesFound.AddRange(Directory.GetFiles(searchFolder, String.Format("*.{0}", filter), searchOption));
            }
            return filesFound.ToArray();
        }

        public static int CheckOccurrences(string str1, string pattern)
        {
            int count = 0;
            int a = 0;

            while ((a = str1.IndexOf(pattern, a)) != -1)
            {
                a += pattern.Length;
                count++;
            }
            return count;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        

        private void firefoxButton1_Click(object sender, EventArgs e)
        {

            string backup_path = @"C:\Users\UserName\Desktop\Facebook 2019\";
            string facebook_name = "Rashan Hasaranga";

            int sent_messages_count = 0;
            int total_messages_count = 0;
            int received_messages_count = 0;
            int unread_messages = 0;
            double messages_responce_rate = 0.0;
            int attachments_imgs = 0;
            int attachments_vids = 0;
            int reacts_total = 0;
            String searchFolder = backup_path + @"messages\inbox";
            var filters = new String[] { "json" };
            var json_files = GetFilesFrom(searchFolder, filters, true);

            string json_text = "";

            //messages
            foreach (string json_file in json_files)
            {
                json_text = "";
                json_text = System.IO.File.ReadAllText(@json_file);

                sent_messages_count += CheckOccurrences(json_text, "\"sender_name\": \"" + facebook_name  + "\"");
                total_messages_count += CheckOccurrences(json_text, "\"timestamp_ms\": ");
                received_messages_count = total_messages_count - sent_messages_count;
                if (CheckOccurrences(json_text, "\"sender_name\": \"" + facebook_name + "\"") == 0) { unread_messages++; }
                messages_responce_rate = 100 - (unread_messages * 100.0 / json_files.Length);


                lbl_total_threads.Text = json_files.Length.ToString();
                lbl_sent_count.Text = sent_messages_count.ToString();
                lbl_rec_count.Text = received_messages_count.ToString();
                lbl_total_msgs.Text = total_messages_count.ToString();
                lbl_unread_count.Text = unread_messages.ToString();
                lbl_resp_rate.Text = messages_responce_rate.ToString("0.00") + "%";
            }

            //attachments
            filters = new String[] { "jpg", "jpeg", "png" };
            var img_files = GetFilesFrom(searchFolder, filters, true);
            attachments_imgs = img_files.Length;
            lbl_attach_imgs.Text = attachments_imgs.ToString();

            filters = new String[] { "mp4", "avi", "webm" };
            var vid_files = GetFilesFrom(searchFolder, filters, true);
            attachments_vids = vid_files.Length;
            lbl_attach_vids.Text = attachments_vids.ToString();

            //friends
            json_text = System.IO.File.ReadAllText(backup_path + @"friends\friends.json");
            lbl_new_friends.Text = CheckOccurrences(json_text, "\"name\": ").ToString();

            //removed friends
            json_text = System.IO.File.ReadAllText(backup_path + @"friends\removed_friends.json");
            lbl_removed.Text = CheckOccurrences(json_text, "\"name\": ").ToString();

            //new posts
            json_text = System.IO.File.ReadAllText(backup_path + @"posts\your_posts_1.json");
            lbl_new_posts.Text = CheckOccurrences(json_text, "\"timestamp\": ").ToString();

            //reactions
            json_text = System.IO.File.ReadAllText(backup_path + @"likes_and_reactions\posts_and_comments.json");
            lbl_likes.Text = CheckOccurrences(json_text, "\"LIKE\"").ToString();
            lbl_loves.Text = CheckOccurrences(json_text, "\"LOVE\"").ToString();
            lbl_wows.Text = CheckOccurrences(json_text, "\"WOW\"").ToString();
            lbl_haha.Text = CheckOccurrences(json_text, "\"HAHA\"").ToString();
            lbl_sads.Text = CheckOccurrences(json_text, "\"SORRY\"").ToString();
            lbl_angry.Text = CheckOccurrences(json_text, "\"ANGER\"").ToString();
            reacts_total = Convert.ToInt16(lbl_likes.Text) + Convert.ToInt16(lbl_loves.Text) + Convert.ToInt16(lbl_wows.Text) + Convert.ToInt16(lbl_haha.Text) + Convert.ToInt16(lbl_sads.Text) + Convert.ToInt16(lbl_angry.Text);

            lbl_n1.Text = lbl_new_friends.Text + "\nNew friends added";
            lbl_n2.Text = lbl_removed.Text + "\nDickheads removed";
            lbl_n3.Text = lbl_total_threads.Text + "\nNew messages";
            lbl_n4.Text = reacts_total.ToString() + "\nNew reacts";
            lbl_n5.Text = lbl_new_posts.Text.ToString() + "\nNew posts";

            //chart
            chart1.Series.Clear();
            chart1.Legends.Clear();


            //Add a new Legend(if needed) and do some formating
            chart1.Legends.Add("My Reacts");
            chart1.Legends[0].LegendStyle = LegendStyle.Table;
            //chart1.Legends[0].Docking = Docking.Bottom;
            //chart1.Legends[0].Alignment = StringAlignment.Center;
            chart1.Legends[0].Title = "My Reacts";
            chart1.Legends[0].BorderColor = Color.Black;

            //Add a new chart-series
            string seriesname = "My Reacts";
            chart1.Series.Add(seriesname);
            //set the chart-type to "Pie"
            chart1.Series[seriesname].ChartType = SeriesChartType.Pie;
            chart1.Series[seriesname]["PieLabelStyle"] = "Disabled";

            //Add some datapoints so the series. in this case you can pass the values to this method
            chart1.Series[seriesname].Points.AddXY("Like", Convert.ToDouble(lbl_likes.Text));
            chart1.Series[seriesname].Points.AddXY("Love", Convert.ToDouble(lbl_loves.Text));
            chart1.Series[seriesname].Points.AddXY("Wow", Convert.ToDouble(lbl_wows.Text));
            chart1.Series[seriesname].Points.AddXY("Haha", Convert.ToDouble(lbl_haha.Text));
            chart1.Series[seriesname].Points.AddXY("Sad", Convert.ToDouble(lbl_sads.Text));
            chart1.Series[seriesname].Points.AddXY("Angry", Convert.ToDouble(lbl_angry.Text));
        }
    }
}
