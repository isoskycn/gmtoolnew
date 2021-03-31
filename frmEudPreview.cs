using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gmtoolNew
{
    public partial class frmEudPreview : Form
    {
        public frmEudPreview()
        {
            InitializeComponent();
        }

        public frmEudPreview(string eud_type,string name,string card_id,string level,string luck,string initLife,string initPhyAtkMin, string initPhyAtkMax,string initPhyDef,string initMagicAtkMin, string initMagicAtkMax,string initMagicDef,string growLife,string growPhyAtkMin,string growPhyAtkMax,string growPhyDef,string growMagicAtkMin,string growMagicAtkMax,string growMagicDef,string rebornTimes,string rebornLmitAdd)
        {
            InitializeComponent();
            //透明
            picOff.Parent = picBG;
            txtStarLev.Parent = picBG;
            labReborns.Parent = picBG;
            labRebornTime.Parent = picBG;
            labStar_lev.Parent = picBG;
            labStarLev.Parent = picBG;
            labName.Parent = picBG;
            labCard_id.Parent = picBG;
            labLevel.Parent = picBG;
            labLuck.Parent = picBG;

            labInitial_life.Parent = picBG;
            labInitial_phy.Parent = picBG;
            labInitial_def.Parent = picBG;
            labInitial_magic.Parent = picBG;
            labInitial_def2.Parent = picBG;

            labLife.Parent = picBG;
            labPhyAtk.Parent = picBG;
            labDef.Parent = picBG;
            labMagicAtk.Parent = picBG;
            labMagicDef.Parent = picBG;

            this.Tag = eud_type;
            labReborns.Tag = rebornTimes;
            labRebornTime.Tag = rebornLmitAdd;
            //初始属性
            labName.Text = name;
            labCard_id.Text = card_id;
            labLevel.Text = level;
            labLuck.Text = luck;
            labInitial_life.Text = initLife;
            labInitial_phy.Text = String.Format("{0}/{1}", initPhyAtkMin, initPhyAtkMax);
            labInitial_def.Text = initPhyDef;
            labInitial_magic.Text = String.Format("{0}/{1}", initMagicAtkMin, initMagicAtkMax);
            labInitial_def2.Text = initMagicDef;
            //计算属性
            labLife.Text =Moyu.GetEudAttr(Convert.ToInt32(initLife), Convert.ToInt32(growLife), Convert.ToInt32(level),true).ToString();

            string phyAtkMin = Moyu.GetEudAttr(Convert.ToInt32(initPhyAtkMin), Convert.ToInt32(growPhyAtkMin), Convert.ToInt32(level)).ToString();
            string phyAtkMax = Moyu.GetEudAttr(Convert.ToInt32(initPhyAtkMax), Convert.ToInt32(growPhyAtkMax), Convert.ToInt32(level)).ToString();
            labPhyAtk.Text = String.Format("{0}/{1}", phyAtkMin, phyAtkMax);
            string def = Moyu.GetEudAttr(Convert.ToInt32(initPhyDef), Convert.ToInt32(growPhyDef), Convert.ToInt32(level)).ToString();
            labDef.Text = def;

            string magicAtkMin = Moyu.GetEudAttr(Convert.ToInt32(initMagicAtkMin), Convert.ToInt32(growMagicAtkMin), Convert.ToInt32(level)).ToString();
            string magicAtkMax = Moyu.GetEudAttr(Convert.ToInt32(initMagicAtkMax), Convert.ToInt32(growMagicAtkMax), Convert.ToInt32(level)).ToString();
            labMagicAtk.Text = String.Format("{0}/{1}", magicAtkMin, magicAtkMax);
            string magicDef = Moyu.GetEudAttr(Convert.ToInt32(initMagicDef), Convert.ToInt32(growMagicDef), Convert.ToInt32(level)).ToString();
            labMagicDef.Text = magicDef;

            //计算评分
            string sql = String.Format("select reborn_limit,rarity from cq_grade  where id='{0}';", this.Tag);
            DataTable dt = MySqlHelper.GetDataTable(sql);
            int rebornLimit = Convert.ToInt32(dt.Rows[0]["reborn_limit"]);
            labRebornTime.Text = String.Format("{0}/{1}(+{2})", labReborns.Tag, rebornLimit, labRebornTime.Tag);
            int type = Convert.ToInt32(this.Tag);
            //转世
            int rarityStarLev = Convert.ToInt32(dt.Rows[0]["rarity"]);
            int rebornStarLev = Convert.ToInt32(labReborns.Tag) > rebornLimit ? rebornLimit : Convert.ToInt32(labReborns.Tag);
            //雷属性

            //初始
            int initLifeStarLev = Moyu.GetEudInitAttrStarLev(type, 0, Convert.ToInt32(initLife));
            int initPhyAtkMinStarLev = Moyu.GetEudInitAttrStarLev(type, 1, Convert.ToInt32(initPhyAtkMin));
            int initPhyAtkMaxStarLev = Moyu.GetEudInitAttrStarLev(type, 2, Convert.ToInt32(initPhyAtkMax));
            int initPhyDefStarLev = Moyu.GetEudInitAttrStarLev(type, 3, Convert.ToInt32(initPhyDef));

            int initMgcAtkMinStarLev = Moyu.GetEudInitAttrStarLev(type, 4, Convert.ToInt32(initMagicAtkMin));
            int initMgcAtkMaxStarLev = Moyu.GetEudInitAttrStarLev(type, 5, Convert.ToInt32(initMagicAtkMax));
            int initMgcDefStarLev = Moyu.GetEudInitAttrStarLev(type, 6, Convert.ToInt32(initMagicDef));


            //成长
            int growLifeStarLev = Moyu.GetEudInitAttrStarLev(type, 7, Convert.ToInt32(growLife));
            int growPhyAtkMinStarLev = Moyu.GetEudGrowAttrStarLev(type, 1, Convert.ToInt32(growPhyAtkMin));
            int growPhyAtkMaxStarLev = Moyu.GetEudGrowAttrStarLev(type, 2, Convert.ToInt32(growPhyAtkMax));
            int growPhyDefStarLev = Moyu.GetEudGrowAttrStarLev(type, 3, Convert.ToInt32(growPhyDef));

            int growMgcAtkMinStarLev = Moyu.GetEudGrowAttrStarLev(type, 4, Convert.ToInt32(growMagicAtkMin));
            int growMgcAtkMaxStarLev = Moyu.GetEudGrowAttrStarLev(type, 5, Convert.ToInt32(growMagicAtkMax));
            int growMgcDefStarLev = Moyu.GetEudGrowAttrStarLev(type, 6, Convert.ToInt32(growMagicDef));

            txtStarLev.Text = (rarityStarLev + rebornStarLev + initLifeStarLev + initPhyAtkMinStarLev + initPhyAtkMaxStarLev + initPhyDefStarLev + initMgcAtkMinStarLev
                + initMgcAtkMaxStarLev + initMgcDefStarLev + growLifeStarLev + growPhyAtkMinStarLev + growPhyAtkMaxStarLev + growPhyDefStarLev + growMgcAtkMinStarLev + growMgcAtkMaxStarLev + growMgcDefStarLev).ToString();
        }

        private void picOff_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
        }

        private void frmEudPreview_Load(object sender, EventArgs e)
        {
        }
    }
}
