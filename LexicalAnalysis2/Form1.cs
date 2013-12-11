using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace LexicalAnalysis2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        static SystemSymbol ss = new SystemSymbol();
        static FileOperation fo = new FileOperation();
        static List<Formula> table = new List<Formula> { };
        static JudgeChar jc = new JudgeChar();
        static string buffer = "";
        int count, length, count2;
        string[] word ={"N+N", "N-N", "N*N", "N/N", ")N(", "i" };
        List<string> parts = new List<string> { };
        public void initialize()
        {
            ss.key = fo.readFile("Keys.txt");
            ss.opt = fo.readFile("Operators.txt");
            ss.prept = fo.readFile("Prepositions.txt");
            ss.reln = fo.readFile("Relations.txt");
            string sys = "";
            int j;
            sys += string.Format("系统关键字({0}个):\r\n", ss.key.Length);
            for (j = 0; j < ss.key.Length; j++)
                sys += ss.key[j] + Environment.NewLine;

            sys += string.Format("系统运算符({0}个):\r\n", ss.opt.Length);
            for (j = 0; j < ss.opt.Length; j++)
                sys += ss.opt[j] + Environment.NewLine;

            sys += string.Format("系统关系符({0}个):\r\n", ss.reln.Length);
            for (j = 0; j < ss.reln.Length; j++)
                sys += ss.reln[j] + Environment.NewLine;

            sys += string.Format("系统界符({0}个):\r\n", ss.prept.Length);
            for (j = 0; j < ss.prept.Length; j++)
                sys += ss.prept[j] + Environment.NewLine;
            fo.writeStringToFile("SystemSymbols.txt", sys);
        }
        private void load(object sender, EventArgs e)
        {
            this.initialize();
            count = 0;
            this.groupBox1.Enabled = false;
            this.menuStrip1.Items[1].Enabled = false;
        }
        public void recordInTable(string type, string word, string label)
        {
            Formula f = new Formula(type, word, label);
            for(int i = 0;i < table.Count;++i)
            {
                if(table[i].value == word)
                    return;
            }
            table.Add(f);
            fo.writeStringToFile("BiTable.txt", f.convertToString());
        }
        public string preProcess(string oldCode,string toFile)
        {
            int flag = 0;
            int l = oldCode.Length;
            char[] temp = new char[2000];
            int i = 0, j = 0;
            char oldChar = '\0', curChar;
            int isNote = 0;
            while (j < l)
            {
                curChar = oldCode[j++];
                switch (isNote)
                {
                    case 0:
                        if (oldChar == '/' && curChar == '*')
                        {
                            --i;
                            isNote = 1;
                        }
                        else if (oldChar == '/' && curChar == '/')
                        {
                            --i;
                            isNote = 2;
                        }
                        else
                        {
                            if (oldChar == '\\' && curChar == '\r')
                            {
                                --i;
                                flag = 1;
                            }
                            else
                            {
                                if (curChar == '\t' || curChar == '\r' || curChar == '\n')
                                    curChar = ' ';
                                if (oldChar == ' ' && curChar == ' ')
                                    curChar = ' ';
                                if (oldChar == '\n' && curChar == '\n')
                                    curChar = ' ';
                                if (oldChar == '\r' && curChar == '\r')
                                    curChar = ' ';
                                if (flag != 1)
                                {
                                    temp[i++] = curChar;
                                }
                                else
                                {
                                    flag = 0;
                                }
                            }
                        }
                        break;
                    case 1:
                        if (oldChar == '*' && curChar == '/')
                        {
                            isNote = 0;
                        }
                        break;
                    case 2:
                        if (curChar == '\n')
                            isNote = 0;
                        break;
                    default: break;
                }
                oldChar = curChar;
            }
            for (int s = 0; temp[s] != '\0'; s++)
            {
                buffer += temp[s];
            }
            fo.writeStringToFile(toFile, buffer);
            buffer += buffer[buffer.Length - 1].ToString();
            length = buffer.Length;
            return buffer;
        }
        
        public char spaceProcess(char c)
        {
            while (c == ' ')
            {
                c = buffer[count++];
            }
            return c;
        }
        public char letterProcess(char c)
        {
            string letter = "";
            string temp = "";
	        int i,j;
            while (jc.isAlphabet(c) && count < length)
	        {
                letter += c;
		        c = buffer[count++];
	        }
            if (ss.searchWord(1, letter) && count < length)
            {
                recordInTable("关键字", letter, "K");
                if (letter == "goto" && count < length)
                {
                    c = spaceProcess(c);
                    while (jc.isAlphaOrDigit(c) && count < length)
                    {
                        temp += c;
                        c = buffer[count++];
                    }
                    recordInTable("标号", temp, "L");
                }
            }
            else
            {
                if (c == ':' && count < length)
                {
                    recordInTable("标号", letter, "L");
                    c = buffer[count++];
                }
                else
                {
                    recordInTable("标识符", letter, "B");
                }
            }
            return c;
        }
        public char digitProcess(char c)
        {
            string num = "";
	        const int i = 0;
            while (jc.isDigit(c) && count < length)
	        {
		        num += c;
		        c = buffer[count++];
	        }
	        if(jc.isAlphabet(c))
	        {
                while (jc.isAlphabet(c) && count < length)
		        {
			        num += c;
			        c = buffer[count++];
		        }
		        recordInTable("非法符号",num,"N");
		        return c;
	        }
            else if (c == ':')
            {
                recordInTable("标号", num, "L");
                return c;
            }
            else
            {
                recordInTable("常数", num, "C");
                return c;
            }
        }
        public char otherProcess(char c)
        {
            string others = "";
	        if(c == '(')
	        {
		        others += c;
		        c = buffer[count++];
	        }
            while (jc.isAlphaOrDigit(c) == false && c != ' ' && c != '(' && c != ')' && count < length)
	        {
		        others += c;
		        c = buffer[count++];
	        }
            if (c == ')' && count < length)
	        {
		        others += c;
		        c = buffer[count++];
	        }
	        if(ss.searchWord(3,others))
	        {
		        recordInTable("界符",others,"P");
	        }
	        else if(ss.searchWord(2,others))
	        {
		        recordInTable("运算符",others,"O");
	        }
	        else if(ss.searchWord(4,others))
	        {
		        recordInTable("关系符",others,"R");
	        }
	        else
	        {
		        recordInTable("非法符号",others,"N");
	        }
	        return c;
        }
        public bool compiles()
        {
            try
            {
                string code = this.textBox1.Text;
                buffer = preProcess(code, "PreProcess.txt");
                char c = buffer[count++];
                while (count < buffer.Length)
                {
                    c = spaceProcess(c);
                    if (jc.isAlphabet(c))
                    {
                        c = letterProcess(c);
                    }
                    else
                    {
                        if (jc.isDigit(c))
                        {
                            c = digitProcess(c);
                        }
                        else
                        {
                            c = otherProcess(c);
                        }
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        private void compile_Click(object sender, EventArgs e)
        {
            if (this.compiles())
            {
                this.groupBox1.Enabled = true;
                this.menuStrip1.Items[1].Enabled = true;
                this.codeSymbol();
                MessageBox.Show("词法分析成功！");
            }
            else
            {
                MessageBox.Show("词法分析失败！");
            }
        }

        private void 编译ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.compiles())
            {
                this.groupBox1.Enabled = true;
                this.menuStrip1.Items[1].Enabled = true;
                MessageBox.Show("词法分析成功！");
            }
            else
            {
                MessageBox.Show("词法分析失败！");
            }
        }

        private void 预处理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string code = this.textBox1.Text;
            buffer = preProcess(code, "PreProcess.txt");
            MessageBox.Show("预处理成功！");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.textBox2.Text = fo.readAllText("SystemSymbols.txt");
            this.tabControl1.SelectedIndex = 1;
            this.label1.Text = "系统符号表如下：";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.textBox2.Text = fo.readAllText("PreProcess.txt");
            this.tabControl1.SelectedIndex = 1;
            this.label1.Text = "预处理结果如下：";
        }
        public void codeSymbol()
        {
            //quicksort(table.ToArray(), 0, table.Count - 1);
            for (int i = 0; i < table.Count - 1; i++)
            {
                for (int j = 0; j < table.Count - 1 - i; j++)
                {
                    if(string.Compare(table[j].label,table[j+1].label) > 0)
                    {
                        Formula f = table[j];
                        table[j] = table[j + 1];
                        table[j + 1] = f;
                    }
                }
            }
            string s = "";
            for(int v = 0;v < table.Count;++v)
            {
                s += String.Format("{0}  {1}\r\n", table[v].type, table[v].value);
            }
            fo.writeStringToFile("Symbols.txt", s);
        }
        public void quicksort(Formula[] a,int l,int h)
        {
           if (l>=h)return ;
           int j = h;
           string key;
           int i = l; key = a[i].label;
           while(i<j)
             {
                while(i<j&&string.Compare(a[j].label,key) > 0)j--;
                if (i<j) a[i++]=a[j];
                while (i < j && string.Compare(a[j].label, key) < 0) i++;
                if (i<j) a[j--]=a[i];
             }
            a[i].label=key;
            if (l<i-1)
                quicksort(a,l,i-1);
            if (i+1<h)
                quicksort(a,i+1,h);
 
        }
        private void button6_Click(object sender, EventArgs e)
        {
            this.textBox2.Text = fo.readAllText("BiTable.txt");
            this.tabControl1.SelectedIndex = 1;
            this.label1.Text = "二元式如下：";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.textBox2.Text = fo.readAllText("Symbols.txt");
            this.tabControl1.SelectedIndex = 1;
            this.label1.Text = "程序符号表如下：";
        }
        public void openFile()
        {
            OpenFileDialog fileDialog = new OpenFileDialog() { Title = "请选择文件", Filter = "文本文件|*.txt|C#文件|*.cs|所有文件|*.*" };
            if (fileDialog.ShowDialog() == DialogResult.OK)
            { 
                string file = fileDialog.FileName;
                string extendedName = Path.GetExtension(file);
                string filePath = file;
                MessageBox.Show(filePath);
                this.textBox1.Text = String.Format("/*{0}*/\r\n{1}", file, fo.readAllText(filePath));
                this.tabControl1.SelectedIndex = 0;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.openFile();
        }

        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.openFile();
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void 系统符号表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.textBox2.Text = fo.readAllText("SystemSymbols.txt");
            this.tabControl1.SelectedIndex = 1;
            this.label1.Text = "系统符号表如下：";
        }

        private void 预处理结果ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.textBox2.Text = fo.readAllText("PreProcess.txt");
            this.tabControl1.SelectedIndex = 1;
            this.label1.Text = "预处理结果如下：";
        }

        private void 二元式ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.textBox2.Text = fo.readAllText("BiTable.txt");
            this.tabControl1.SelectedIndex = 1;
            this.label1.Text = "二元式如下：";
        }

        private void 程序符号表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.codeSymbol();
            this.textBox2.Text = fo.readAllText("Symbols.txt");
            this.tabControl1.SelectedIndex = 1;
            this.label1.Text = "程序符号表如下：";
        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutMe ab = new AboutMe();
            ab.Show();
        }
        public string GetStackValue(Stack<char> s)
        {
            string tmp = "";
            for (int i = s.Count - 1; i >= 0; --i)
            {
                tmp += s.ElementAt(i);
            }
            return tmp;
        }

        public bool IsOpt(char c)
        {
            if (c == '+' || c == '-' || c == '*' || c == '/' || c == '(' || c == ')')
                return true;
            else return false;
        }
        private string Trans2Syn(string s)
        {
            JudgeChar jc = new JudgeChar();
            Stack<char> ts = new Stack<char> { };
            for (int i = 0; i < s.Length; ++i)
            {
                if (jc.isDigit(s[i]))
                {
                    if (ts.Count == 0 || ts.Peek() != 'i')
                    {
                        ts.Push('i');
                    }
                }
                else
                {
                    ts.Push(s[i]);
                }
            }
            string tmp = "";
            for (int i = ts.Count - 1; i >= 0;--i )
            {
                tmp += ts.ElementAt(i);
            }
            return tmp;
        }
        public bool CanStackRule(Stack<char> s)
        {
            if (s.Count >= 3)
            {
                int k = 0;
                string tmp = "";
                for (int i = 0; k < 3; ++i,++k)
                {
                    tmp += s.ElementAt(i);
                }
                if (word.Contains(tmp))
                {
                    return true;
                }
            }
            return false;
        }
         string GetALine(Line l)
        {
            return string.Format("{0}        {1}        {2}        {3}\r\n", l.step, l.symbolStack, l.inputStack, l.opt);
        }
         void RecordInLine(int num, string s, string i, string o,List<Line>ll)
        {
            Line l = new Line() { step = num, symbolStack = s, inputStack = i, opt = o };
            ll.Add(l);
        }
        public string Rule(string s)
        {
            for (int i = 0; i < word.Length; ++i)
            {
                if (s == word[i])
                {
                    s = "N";
                    return s;
                }
            }
            return s;
        }
        public void Analyse(string s)
        {
            //建立类与栈
            List<Line> aLine = new List<Line> { };
            Stack<char> ss = new Stack<char> { };
            int c = 0;
            ss.Push('#');
            string tmp = Trans2Syn(s);
            tmp += "#";
            for (int i = 0;i < tmp.Length; )
            {
                c++;
                int a = PublicVariables.Char2Int(ss.Peek());
                int b = PublicVariables.Char2Int(tmp[i]);
                //优先级满足
                if (PublicVariables.matrix[a, b] < 0)
                {
                    //进栈并记录
                    ss.Push(tmp[i]);
                    RecordInLine(i, this.GetStackValue(ss), (ss.Peek() == '#')&&(ss.Count > 1)?(""):tmp.Substring(i,tmp.Length - i), "进栈", aLine);
                    ++i;
                    continue;
                }
                else
                {
                    //如果三个可以规约，则规约并记录
                    if (this.CanStackRule(ss))
                    {
                        string tmps = "";
                        tmps += ss.Pop();
                        tmps += ss.Pop();
                        tmps += ss.Pop();
                        tmps = Rule(tmps);
                        for (int j = 0; j < tmps.Length; ++j)
                        {
                            ss.Push(tmps[j]);
                        }
                        RecordInLine(c, this.GetStackValue(ss), (ss.Peek() == '#') && (ss.Count > 1) ? ("") : tmp.Substring(i, tmp.Length - i), "规约", aLine);
                    }
                    //如果一个可以规约，则规约并记录
                    else if (word.Contains(ss.Peek().ToString()))
                    {
                        string tmps = "";
                        tmps += ss.Pop();
                        tmps = Rule(tmps);
                        for (int j = 0; j < tmps.Length; ++j)
                        {
                            ss.Push(tmps[j]);
                        }
                        RecordInLine(c, this.GetStackValue(ss), (ss.Peek() == '#') && (ss.Count > 1) ? ("") : tmp.Substring(i, tmp.Length - i), "规约", aLine);
                    }
                    else
                    {
                        //进栈并记录
                        ss.Push(tmp[i]);
                        RecordInLine(c, this.GetStackValue(ss), (ss.Peek() == '#') && (ss.Count > 1) ? ("") : tmp.Substring(i, tmp.Length - i), "进栈", aLine);
                        ++i;
                    }
                }
            }
            for (int i = 0; i < aLine.Count; ++i)
            {
                //写到文件里
                fo.writeStringToFile("Rules.txt",this.GetALine(aLine[i]));
                //加到datagridview中
                ruleDgv.Rows.Add(i, aLine[i].symbolStack, aLine[i].inputStack, aLine[i].opt);
            }
        }
        public bool SynAna(string buf)
        {
            int count2 = 0;
            //循环扫描所有算术表达式
            while (count2 < buf.Length)
            {
                while (buf[count2] != '=' && count2 < buf.Length)
                {
                    count2++;
                    if (count2 == buf.Length)
                        break;
                }
                if (count2 == buf.Length)
                    break;
                JudgeChar jc = new JudgeChar();
                if (jc.isAlphabet(buf[count2 - 1]))
                {
                    count2++;
                    string part = "";
                    //判断是算术表达式
                    while ((buf[count2] >= '0' && buf[count2] <= '9') || this.IsOpt(buf[count2]))
                    {
                        part += buf[count2];
                        count2++;
                    }
                    if (part != "")
                    {
                        //加入数组中
                        parts.Add(part);
                        continue;
                    }
                    else continue;
                }
                else count2++;
            }
            try
            {
                for (int i = 0; i < parts.Count; ++i)
                {
                    //对第i个串分析
                    Analyse(parts[i]);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        //点击语法分析按钮后
        private void button1_Click(object sender, EventArgs e)
        {
            this.ruleDgv.DataSource = null;
            this.ruleDgv.Refresh();
            if (this.compiles())     //如果编译成功
            {
                this.groupBox1.Enabled = true;
                this.menuStrip1.Items[1].Enabled = true;
                FileOperation fo = new FileOperation();
                //foBuffer中存放预处理字符串
                string foBuffer = fo.readAllText("PreProcess.txt");
                //调用语法分析函数
                if (SynAna(foBuffer) == true)
                {
                    MessageBox.Show("语法分析成功！");
                    this.tabControl1.SelectedIndex = 2;
                    return;
                }
                else
                {
                    MessageBox.Show("语法分析失败！");
                    return;
                }
            }
            else
            {
                MessageBox.Show("语法分析失败！");
            }
        }
    }
}
