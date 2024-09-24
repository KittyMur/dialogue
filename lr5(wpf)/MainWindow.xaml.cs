using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace lr5_wpf_
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //TextBox txt;
        //ComboBox answAction;
        //Label lbMessage;
        //Label lbAnswer;
        //Label linkedMsg;

        CDialogue dialogue = new CDialogue();

        //TreeView dlg;

        TreeViewItem selectedMessage = null;
        TreeViewItem selectedAnswer = null;

        public MainWindow()
        {
            InitializeComponent();
            //answAction.Items.Add("none");
            //answAction.Items.Add("open door");
            //answAction.Items.Add("exit");
        }
        private void dlg_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (dlg.SelectedItem == null) return;

            TreeViewItem item = dlg.SelectedItem as TreeViewItem;

            if ((string)item.Tag == "message")
            {
                selectedMessage = item;
                long msgID = (long)selectedMessage.Header;
                txt.Text = dialogue.selectMessage(msgID);
                lbMessage.Content = msgID;
            } else
            {
                    selectedMessage = item.Parent as TreeViewItem;
                    long msgID = (long)selectedMessage.Header;
                    long answID = (long)item.Header;
                    txt.Text = dialogue.selectAnswer(msgID, answID);
                    selectedAnswer = item;
                    lbMessage.Content = msgID;
                    lbAnswer.Content = answID;
                    linkedMsg.Content = dialogue.linkedUID();
            }
        }
        private void AddMsgButton(object sender, RoutedEventArgs e)
        {
            CMessage msg = new CMessage();
            msg.text = txt.Text;

            TreeViewItem item = new TreeViewItem();
            item.Tag = "message";
            item.Header = dialogue.addMessage(msg);
            item.IsExpanded = true;
            item.IsSelected = true;

            dlg.Items.Add(item);
        }
        private void AddAnswerButton(object sender, RoutedEventArgs e)
        {
            CAnswer answ = new CAnswer();
            answ.text = txt.Text;

            TreeViewItem item = new TreeViewItem();
            item.Tag = "answer";
            //if (Dia1.IsChecked == true)
            //    item.Header = dialogue.addAnswer(answ, (answAction.SelectedItem as ComboBoxItem).Content.ToString());

            //if (Dia2.IsChecked == true)
                item.Header = dialogue.addAnswer1(answ, (answAction.SelectedItem as ComboBoxItem).Content.ToString(), (plusStat.SelectedItem as ComboBoxItem).Content.ToString());

            selectedMessage.Items.Add(item);
        }
        private void LinkButton(object sender, RoutedEventArgs e)
        {
            dialogue.connectAnswer((long)selectedMessage.Header);
        }
        private void RemoveButton(object sender, RoutedEventArgs e)
        {
            if (selectedMessage != null) // Если выбран элемент сообщения
            {
                // Удаляем сообщение из диалога
                long msgID = (long)selectedMessage.Header;
                dialogue.removeMessage(msgID);
                // Удаляем выбранный элемент из TreeViewItem
                dlg.Items.Remove(selectedMessage);
                // Сбрасываем выбранный элемент
                selectedMessage = null;
                lbMessage.Content = null;
            }
            else if (selectedAnswer != null) // Если выбран элемент ответа
            {
                // Удаляем ответ из диалога
                long msgID = (long)selectedMessage.Header;
                long answID = (long)selectedAnswer.Header;
                dialogue.removeAnswer(msgID, answID);
                // Удаляем выбранный элемент из TreeViewItem
                selectedMessage.Items.Remove(selectedAnswer);
                // Сбрасываем выбранный элемент
                selectedAnswer = null;
                lbMessage.Content = null;
                lbAnswer.Content = null;
                linkedMsg.Content = null;
            }
        }
        private void SaveButton(object sender, RoutedEventArgs e)
        {
            XmlDocument xmlDoc = new XmlDocument();

            XmlNode rootNode = xmlDoc.CreateElement("messages");
            XmlAttribute attribute = xmlDoc.CreateAttribute("uid");
            attribute.Value = dialogue.getLastUID().ToString();
            rootNode.Attributes.Append(attribute);
            xmlDoc.AppendChild(rootNode);

            foreach (CMessage msg in dialogue.getMessages())
            {
                XmlNode messageNode = xmlDoc.CreateElement("message");

                attribute = xmlDoc.CreateAttribute("uid");
                attribute.Value = msg.msgID.ToString();

                messageNode.Attributes.Append(attribute);
                messageNode.InnerText = msg.text;

                XmlNode answersNode = xmlDoc.CreateElement("answers");

                foreach (CAnswer answ in msg.answers)
                {

                    XmlNode answerNode = xmlDoc.CreateElement("answer");

                    attribute = xmlDoc.CreateAttribute("auid");
                    attribute.Value = answ.answID.ToString();
                    answerNode.Attributes.Append(attribute);
                    attribute = xmlDoc.CreateAttribute("muid");
                    attribute.Value = answ.msgID.ToString();
                    answerNode.Attributes.Append(attribute);
                    attribute = xmlDoc.CreateAttribute("action");
                    attribute.Value = answ.action;
                    answerNode.Attributes.Append(attribute);
                    attribute = xmlDoc.CreateAttribute("stat");
                    attribute.Value = answ.stat;
                    answerNode.Attributes.Append(attribute);

                    answerNode.InnerText = answ.text;
                    answersNode.AppendChild(answerNode);

                }

                messageNode.AppendChild(answersNode);
                rootNode.AppendChild(messageNode);
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.ShowDialog();

            xmlDoc.Save(saveDialog.FileName);
        }
        private void EditButton(object sender, RoutedEventArgs e)
        {
            if (dlg.SelectedItem == null) return;

            TreeViewItem item = dlg.SelectedItem as TreeViewItem;

            if ((string)item.Tag == "message") // Если выбран элемент сообщения
            {
                dialogue.updateMessage(txt.Text);
                //selectedMessage = null;
            }
             else // Если выбран элемент ответа
            {
                dialogue.updateAnswer(txt.Text, (answAction.SelectedItem as ComboBoxItem).Content.ToString());
                //selectedAnswer = null;
            }
        }
        private void dlgClear()
        {

        }
        private void LoadButton(object sender, RoutedEventArgs e)
        {
            OpenFileDialog loadDialog = new OpenFileDialog();
            loadDialog.ShowDialog();
            XmlDocument xmlDoc = new XmlDocument();
            //dlgClear_Click();
            xmlDoc.Load(loadDialog.FileName);
            XmlNode messages = xmlDoc.SelectSingleNode("//messages");
            dialogue.setLastUID(long.Parse(messages.Attributes["uid"].Value));

            XmlNodeList messageNodes = xmlDoc.SelectNodes("//messages/message");

            foreach (XmlNode messageNode in messageNodes)
            {
                CMessage msg = new CMessage();
                msg.text = messageNode.ChildNodes[0].InnerText;
                msg.msgID = long.Parse(messageNode.Attributes["uid"].Value);
                dialogue.loadMessage(msg);

                TreeViewItem item = new TreeViewItem();
                item.Tag = "message";
                item.Header = msg.msgID;
                item.IsExpanded = true;
                item.IsSelected = true;
                dlg.Items.Add(item);
                //////
                selectedMessage = item;
                //////
                foreach (XmlNode answerNode in messageNode.ChildNodes[1].ChildNodes)
                {

                    CAnswer answ = new CAnswer();
                    answ.answID = long.Parse(answerNode.Attributes["auid"].Value);
                    answ.msgID = long.Parse(answerNode.Attributes["muid"].Value);
                    answ.action = answerNode.Attributes["action"].Value;
                    answ.stat = answerNode.Attributes["stat"].Value;
                    answ.text = answerNode.InnerText;

                    dialogue.loadAnswer(answ);

                    item = new TreeViewItem();
                    item.Tag = "answer";
                    item.Header = answ.answID;

                    selectedMessage.Items.Add(item);

                }
            }
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            selectedMessage = null;
            selectedAnswer = null;
            dlg.Items.Clear();
            dialogue.Clear();
            txt.Clear();
        }
    }
}
