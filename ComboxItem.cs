namespace gmtoolNew
{
    //combobox类 用于设置 value
    public class ComboxItem
    {
        private string text;
        private string values;

        public string Text
        {
            get { return this.text; }
            set { this.text = value; }
        }

        public string Values
        {
            get { return this.values; }
            set { this.values = value; }
        }

        public ComboxItem(string _Text, string _Values)
        {
            Text = _Text;
            Values = _Values;
        }


        public override string ToString()
        {
            return Text;
        }
    }

    public class MyList
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public MyList(string k, string v)
        {
            this.Key = k;
            this.Value = v;
        }
    }
}
