using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static BlaBlaInfoBot.Commands;
using static System.Net.Mime.MediaTypeNames;

namespace BlaBlaInfoBot
{
    internal class TextMessageWorker
    {
        //inform Commands
        public CommandAddGoldStih AddGoldStihC;

        public CommandGet GetW;
        public CommandMem MemC ;
        public CommandAdd Add;
        public CommandInfo InfC;
        public CommandEmpty EmptC;
        public CommandStih StihC;
        public CommandGold GoldStihC;
        public CommandScreen ScrinC;
        public CommandStart StartC;
        public CommandMemVP MemVpC;
        public TextMessageWorker(string textMessage)
        {
            GetW = new CommandGet(textMessage);
            MemC = new CommandMem(textMessage);
            Add = new CommandAdd(textMessage);
            InfC = new CommandInfo(textMessage);
            EmptC = new CommandEmpty(textMessage);
            StihC = new CommandStih(textMessage);
            GoldStihC = new CommandGold(textMessage);
            ScrinC = new CommandScreen(textMessage);
            StartC = new CommandStart(textMessage);
            AddGoldStihC = new CommandAddGoldStih(textMessage);
            MemVpC = new CommandMemVP(textMessage);
        }
        public string GetCommand()
        {
            return GetW & Add & MemC & InfC & GoldStihC & StihC & ScrinC & StartC & MemVpC;
        }
    }

    public class FunctionLecal
    {
        Words lecalWords;
        Function func;
        public FunctionLecal(Words lecalWords, Function func)
        {
            this.lecalWords = lecalWords;
            this.func = func;
        }
    }
    public class Function
    {
        string command;
        string description;
        public string Command { get { return command; } }
        public string Description { get { return description; } }

        public Function(string command, string description)
        {
            this.command = command;
            this.description = description;
        }
        
    }
    public class Words : List<Word> 
    {
        public Words(string[] args)
        {
            foreach (string a in args)
            {
                Add(new Word(a)) ;
            }
        }
        public static Words Get(string text,char separator=' ')
        {
            string[] split = text.Split(separator);
            Words w = new Words(split);
            return w;
        }
       
    }
    public class Word
    {
        string text;
        public string Text { get { return text; } }
        public Word(string t)
        {
            this.text = t;
        }
        public static bool operator ==(Word w1, Word w2)
        {
            return w1.text.ToLower() == w2.text.ToLower();
        }
        public static bool operator !=(Word w1, Word w2)
        {
            return !(w1 == w2);
        }

        public string ToString()
        {
            return text;
        }
    }
    public abstract class WordsCommand
    {
        internal string text = "";
        internal Words wrds;
        internal Words answers;
        internal Function func;

        public FunctionLecal Lecals { get; set; }
        public int FirstSelectedWord(string text="")
        {
            //Обработка входящего текста, удаления ненужных символов
            if (text == "") text = this.text;
            text = TextHandler.DeleteChars(text);

            Words wInT = Words.Get(text);
            for (int i = 0; i < wInT.Count(); i++)
            {
                Word w = wInT[i];
                foreach (Word wI in wrds)
                {
                    if (w == wI)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
        public static string operator &(WordsCommand w1, WordsCommand w2)
        {
            string text = (w1.FirstSelectedWord() != -1 ? w1.func.Command : "") + (w2.FirstSelectedWord() != -1 ? w2.func.Command : "");
            return text;
        }
        public static string operator &(string w1, WordsCommand w2)
        {
            string text = w1 + (w2.FirstSelectedWord() != -1 ? w2.func.Command : "");
            return text;
        }
        public string GetRandomAnswer()
        {
            Random r = new Random();
            int i = r.Next(0, answers.Count);
            return answers[i].Text;
        }
    }
    public class CommandGet : WordsCommand
    {
        public CommandGet(string text)
        {
            wrds = Words.Get("get гет получить дай дайте можно");
            func = new Function("/get", "Возвращает необходимый элемент");
            Lecals = new FunctionLecal(wrds, func);
            answers = new Words(new string[] {"Получить? А что?"});
            this.text = text;
        }
    }
    public class CommandMem : WordsCommand
    {
        public CommandMem(string text)
        {
            wrds = Words.Get("mem мем мемас мемс картинка картинку картиночка картиночку пичку пнгшку пичка пнгшка мемчик мемасик мемыч мемась");
            func = new Function("/mem", "Работает с мемами,если нет других параметров то присылает случайный мем");
            Lecals = new FunctionLecal(wrds, func);
            answers = new Words(new string[] { "Вот вам мем","Мем бомба, честно говоря","Вот те нате","Суперская мемическая картинка" });
            this.text = text;
        }

    }
    public class CommandAdd : WordsCommand
    {
        public CommandAdd(string text)
        {
            wrds = Words.Get("добавь добавить добавление add + прибавь внести внеси дприбавить новый новую новая");
            func = new Function("/add", "Добалвяет необходимый элемент");
            Lecals = new FunctionLecal(wrds, func);
            answers = new Words(new string[] { "Оп Оп Оп мемасик", "Вау это что мемчик, ммм кайф","Картиночка? Это деду надо","Это деду нааадо","МЕМ!!!! вот за это" });
            this.text = text;
        }
    }
    public class CommandInfo : WordsCommand
    {
        public CommandInfo(string text)
        {
            wrds = Words.Get("информация информацию информации инфу инф инфо inf info inform");
            func = new Function("/info", " предоставляет информацию о боте");
            Lecals = new FunctionLecal(wrds, func);
            answers = new Words(new string[] { "Информация о боте", "Вот такие у нас значится пироги" });
            this.text = text;
        }
    }
    public class CommandEmpty : WordsCommand
    {
        public CommandEmpty(string text)
        {
            wrds = Words.Get("");
            func = new Function("", "Отсутствие команды, необходима для получение ответа на простой текст");
            Lecals = new FunctionLecal(wrds, func);
            answers = new Words(new string[] { "Не ну это ты дело сказал...", "Чего? Ты себя слышишь вообще?","Что ты несешь?","Ага, полностью согласен" });
            this.text = text;
        }
    }
    public class CommandStih : WordsCommand
    {
        public CommandStih(string text)
        {
            wrds = Words.Get("стих стишок стишочек стишка стишочка стиха стихов стихи");
            func = new Function("/stih", "Предоставляет рандомный стих из библии");
            Lecals = new FunctionLecal(wrds, func);
            answers = new Words(new string[] { "Стишочек из библии", "Вот стишок","Благослови тебя Господь", "Как думаешь это для тебя слово?" });
            this.text = text;
        }
    }
    public class CommandScreen : WordsCommand
    {
        public CommandScreen(string text)
        {
            wrds = Words.Get("скрин скриншот screen screenshot");
            func = new Function("/screen", "Предоставляет Скриншот моего рабочего стола");
            Lecals = new FunctionLecal(wrds, func);
            answers = new Words(new string[] { "Подглядываешь? Злодей(", "А вот незя так подглядывать внаглую", "На смотри на то чем я занимаюсь" });
            this.text = text;
        }
    }
    public class CommandGold : WordsCommand
    {
        public CommandGold(string text)
        {
            wrds = Words.Get("золотой золотые златый золото зол g з");
            func = new Function("/gold", "Предоставляет рандомный стих из библии");
            Lecals = new FunctionLecal(wrds, func);
            answers = new Words(new string[] { "Стишочек из библии", "Вот стишок", "Золотой стих", "Как думаешь это для тебя слово?" });
            this.text = text;
        }
    }
    public class CommandStart : WordsCommand
    {
        public CommandStart(string text)
        {
            wrds = Words.Get("старт start");
            func = new Function("/start", "Показывает стартовое приветствие и настраивает клавиатуру пользователя");
            Lecals = new FunctionLecal(wrds, func);
            answers = new Words(new string[] { "Стартуем! Я сказал стартуем!", "Сейчас я стартану по полной", "Джаст дуит!", "Джарвис готов" });
            this.text = text;
        }
    }
    public class CommandAddGoldStih : WordsCommand
    {
        public CommandAddGoldStih(string text)
        {
            wrds = Words.Get("");
            func = new Function("/add/gold/stih", " Команда необходима для вывода сообщений");
            Lecals = new FunctionLecal(wrds, func);
            answers = new Words(new string[] { "Новый золотой стих? Круто!", "Чтооо? Ты уверен что это золотой стих?", "Пойдёт, хорошо!" });
            this.text = text;
        }
    }
    public class CommandMemVP : WordsCommand
    {
        public CommandMemVP(string text)
        {
            wrds = Words.Get("переговоров вп vp перег пер per переговор");
            func = new Function("/vp", "Для важных переговоров");
            Lecals = new FunctionLecal(wrds, func);
            answers = new Words(new string[] { "Мем для важных переговоров на тему" });
            this.text = text;
        }
    }

    public class Commands : List<TextCommand> 
    {
        static string[] commands = new string[] { "/start", "/get", "/screen", "/stih", "/mem", "/info","/add", "/gold","/vp"};

        public bool IsEmpty { get { return this == Empty; } }
        public bool IsInfo { get { return this == Info || this == GetInfo; } }
        public bool IsScreen { get { return this == Screen || this == GetScreen; } }
        public bool IsStih { get { return this == Stih || this == GetStih; } }
        public bool IsMem { get { return this == Mem || this == GetMem; } }
        public bool IsAddMem { get { return  this == MemAdd; } }
        public bool IsGoldStih { get { return this == GoldStih; } }
        public bool IsAddGoldStih { get { return this == AddGoldStih; } }
        public bool IsStart { get { return this == Start; } }
        public bool IsVP { get { return this == VP || this == MemVP; } }



        public static Commands Empty { get { return new Commands(); } }

        public static Commands MemAdd
        {
            get
            {
                Commands c = new Commands();
                c.Add(new TextCommand(4, commands[4]));
                c.Add(new TextCommand(6, commands[6]));
                return c;
            }
        }
        public static Commands GetScreen
        {
            get
            {
                Commands c = new Commands();
                c.Add(new TextCommand(1, commands[1]));
                c.Add(new TextCommand(2, commands[2]));
                return c;
            }
        }
        public static Commands GetStih
        {
            get
            {
                Commands c = new Commands();
                c.Add(new TextCommand(1, commands[1]));
                c.Add(new TextCommand(3, commands[3]));
                return c;
            }
        }
        public static Commands GetMem
        {
            get
            {
                Commands c = new Commands();
                c.Add(new TextCommand(1, commands[1]));
                c.Add(new TextCommand(4, commands[4]));
                return c;
            }
        }
        public static Commands GetInfo
        {
            get
            {
                Commands c = new Commands();
                c.Add(new TextCommand(1, commands[1]));
                c.Add(new TextCommand(5, commands[5]));
                return c;
            }
        }
        public static Commands AddGoldStih 
        {
            get
            {
                Commands c = new Commands();
                c.Add(new TextCommand(3, commands[3]));
                c.Add(new TextCommand(6, commands[6]));
                c.Add(new TextCommand(7, commands[7]));
                return c;
            }
        }

        public static Commands Screen
        {
            get
            {
                Commands c = new Commands();
                c.Add(new TextCommand(2, commands[2]));
                return c;
            }
        }
        public static Commands GoldStih
        {
            get
            {
                Commands c = new Commands();
                c.Add(new TextCommand(3, commands[3]));
                c.Add(new TextCommand(7, commands[7]));
                return c;
            }
        }
        public static Commands Stih
        {
            get
            {
                Commands c = new Commands();

                c.Add(new TextCommand(3, commands[3]));
                return c;
            }
        }
        public static Commands Mem
        {
            get
            {
                Commands c = new Commands();
                c.Add(new TextCommand(4, commands[4]));
                return c;
            }
        }
        public static Commands Info
        {
            get
            {
                Commands c = new Commands();
                c.Add(new TextCommand(5, commands[5]));
                return c;
            }
        }
        public static Commands Start
        {
            get
            {
                Commands c = new Commands();
                c.Add(new TextCommand(0, commands[0]));
                return c;
            }
        }
        public static Commands VP {
            get
            {
                Commands c = new Commands();
                c.Add(new TextCommand(8, commands[8]));
                return c;
            }
        }
        public static Commands MemVP
        {
            get
            {
                Commands c = new Commands();
                c.Add(new TextCommand(4, commands[4]));
                c.Add(new TextCommand(8, commands[8]));
                return c;
            }
        }

        public static Commands Selected(string text)
        {
            Commands cmnds = new Commands();
            for (int i = 0; i < commands.Length; i++)
            {
                string s = commands[i];
                int index = text.IndexOf(s);
                if (index != -1) cmnds.Add(new TextCommand(i,s));
            }
            foreach (string s in commands)
            {
               
            }
            return cmnds;
        }
        public static bool operator ==(Commands c1, Commands c2)
        {
            if (c1.Count != c2.Count) return false;
            else
            {
                for (int i = 0; i < c1.Count; i++)
                {
                    if (c1[i] != c2[i]) return false;
                }
                return true;
            } 
        }
        public static bool operator !=(Commands c1, Commands c2)
        {
            return false;
        }
        public string[] ToStringArray()
        {
            List<string> arr = new List<string>();
            foreach (TextCommand a in this)
            {
                arr.Add(a.Text);
            }
            return arr.ToArray();
        }
        public string ToString()
        {
            string ret = "";
            foreach (TextCommand com in this)
            {
                ret += com.Text;
            }
            return ret;
        }
    }
    public class TextCommand
    {
        int id;
        string textCommand;

        public int Id { get { return id; } }
        public string Text { get { return textCommand; } }

        public static bool operator ==(TextCommand c1, TextCommand c2)
        {
            return c1.Id == c2.Id;
        }
        public static bool operator !=(TextCommand c1, TextCommand c2)
        {
            return !(c1 == c2);
        }

        public TextCommand(int id, string textCommand)
        {
            this.id = id;
            this.textCommand = textCommand;
        }
    }
    public class TextHandler
    {
        public static string DeleteChars(string text)
        {
            var charsToRemove = new string[] { "@", ",", ".", ";", "'", "/", "\\" };
            foreach (var c in charsToRemove)
            {
                text = text.Replace(c, string.Empty);
            }
            return text;
        }

    }
}
