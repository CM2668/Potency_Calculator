namespace FFTTRPGCalculator
{
	public partial class PotencyCalculator:Form
	{
		int cooldownTrackers = 0;
		public PotencyCalculator() { InitializeComponent(); }
		public void ClearPage ()
		{
			statsPage.Visible = false;
			actionPage.Visible = false;
			defensePage.Visible = false;
			spiritPage.Visible = false;
		}
		private void PotencyCalculator_Load(object sender,EventArgs e)
		{
			ClearPage(); statsPage.Visible = true;
			UpdateStatLabels(sender, e);
			UpdateEffect(sender, e);
		}
		private void FocusPage(object sender,EventArgs e) { }
		private async void save_Click(object sender,EventArgs e)
		{
			string[] lines = new string[13];

			lines[0] = level.Text;
			if(tankCheck.Checked)
				lines[1] = "tank";
			else if(casterCheck.Checked)
				lines[1] = "caster";
			else if(meleeCheck.Checked)
				lines[1] = "melee";
			else
				lines[1] = "ranged";
			lines[2] = stat1Text.Text;
			lines[3] = stat1Value.Text;
			lines[4] = stat2Text.Text;
			lines[5] = stat2Value.Text;
			lines[6] = defValue.Text;
			lines[7] = sprValue.Text;
			if(stat1Check.Checked)
				lines[8] = "stat 1";
			else
				lines[8] = "stat 2";
			lines[9] = crit.Text;
			lines[10] = defenseLevel.Text;
			lines[11] = spiritLevel.Text;
			lines[12] = cooldownTrackers.ToString();

			await File.WriteAllLinesAsync("character.txt", lines);
		}

		private void load_Click(object sender,EventArgs e)
		{
			string[] lines = new string[13]; int i = 0;
			foreach(string line in File.ReadLines("character.txt"))
				lines[i++] = line;
			level.Text = lines[0];
			if(lines[1] == "tank")
				tankCheck.Checked = true;
			else if(lines[1] == "caster")
				casterCheck.Checked = true;
			else if(lines[1] == "melee")
				meleeCheck.Checked = true;
			else
				rangedCheck.Checked = true;
			stat1Text.Text = lines[2];
			stat1Value.Text = lines[3];
			stat2Text.Text = lines[4];
			stat2Value.Text = lines[5];
			defValue.Text = lines[6];
			sprValue.Text = lines[7];
			if(lines[8] == "stat 1")
				stat1Check.Checked = true;
			else
			{
				stat2Check.Checked = true;
				statLabel.Text = stat2Check.Text;
				stat.Text = stat2Value.Text;
			}
			crit.Text = lines[9];
			defenseLevel.Text = lines[10];
			spiritLevel.Text = lines[11];
			cooldownTrackers = int.Parse(lines[12]);
			for (int j = 0; j < cooldownTrackers; j++)
				AddTracker();
		}

		private int OnlyNumbers(string s, bool canBeNegative)
		{
			char[] chars = s.ToCharArray();
			bool negative = false;
			for(int i = 0; i < chars.Length; i++)
				if(!Char.IsNumber(chars[i]))
				{
					if(chars[i] == '-' && canBeNegative)
						negative = true;
					chars[i] = ',';
				}
			s = "";
			for(int i = 0; i < chars.Length; i++)
				s += chars[i];
			string[] temp = s.Split(",");
			s = "";
			for(int i = 0; i < temp.Length; i++)
				s += temp[i];
			if(negative)
				return -int.Parse(s);
			else
				return int.Parse(s);
		}

		#region Stats
		private void stats_Click(object sender,EventArgs e) {ClearPage(); statsPage.Visible = true;}
		private void action_Click(object sender,EventArgs e) {ClearPage(); actionPage.Visible = true;}
		private void defense_Click(object sender,EventArgs e) {ClearPage(); defensePage.Visible = true;}
		private void spirit_Click(object sender,EventArgs e) {ClearPage(); spiritPage.Visible = true;}
		private void tankCheck_CheckedChanged(object sender,EventArgs e)
		{
			if(tankCheck.Checked)
			{
				casterCheck.Checked = false;
				meleeCheck.Checked = false;
				rangedCheck.Checked = false;
			}
			UpdateFinalDamage();
			UpdateFinalSpirit();
		}
		private void casterCheck_CheckedChanged(object sender,EventArgs e)
		{
			if(casterCheck.Checked)
			{
				tankCheck.Checked = false;
				meleeCheck.Checked = false;
				rangedCheck.Checked = false;
			}
			UpdateFinalDamage();
			UpdateFinalSpirit();
		}
		private void meleeCheck_CheckedChanged(object sender,EventArgs e)
		{
			if(meleeCheck.Checked)
			{
				tankCheck.Checked = false;
				casterCheck.Checked = false;
				rangedCheck.Checked = false;
			}
			UpdateFinalDamage();
			UpdateFinalSpirit();
		}
		private void rangedCheck_CheckedChanged(object sender,EventArgs e)
		{
			if(rangedCheck.Checked)
			{
				tankCheck.Checked = false;
				casterCheck.Checked = false;
				meleeCheck.Checked = false;
			}
			UpdateFinalDamage();
			UpdateFinalSpirit();
		}
		#endregion

		#region Action
		private void UpdateStatLabels(object sender,EventArgs e)
		{
			statLabel.Text = stat1Text.Text;
			stat1Check.Text = stat1Text.Text;
			stat2Check.Text = stat2Text.Text;
		}
		private void stat1Value_TextChanged(object sender,EventArgs e)
		{
			if(stat1Value.Text == "") {
				stat1Value.Text = "0";
				stat1Value.SelectAll();
			}
			if(stat1Check.Checked)
				stat.Text = stat1Value.Text;
		}
		private void stat2Value_TextChanged(object sender,EventArgs e)
		{
			if(stat2Value.Text == "") {
				stat2Value.Text = "0";
				stat2Value.SelectAll();
			}
			if(stat2Check.Checked)
				stat.Text = stat2Value.Text;
		}
		private void defValue_TextChanged(object sender,EventArgs e)
		{ 
			if(defValue.Text == "") {
				defValue.Text = "0";
				defValue.SelectAll();
			}
			defenseStat.Text = defValue.Text;
		}
		private void sprValue_TextChanged(object sender,EventArgs e)
		{ 
			if(sprValue.Text == "") {
				sprValue.Text = "0";
				sprValue.SelectAll();
			}
			spiritStat.Text = sprValue.Text;
		}
		private void UpdateEffect(object sender,EventArgs e)
		{
			if(rollText.Text != "" && potencyText.Text != "" && crit.Text != "")
			{
				int level = OnlyNumbers(this.level.Text, false);
				int stat = OnlyNumbers(this.stat.Text, false);
				double buff = OnlyNumbers(this.buff.Text, true);
				int roll = OnlyNumbers(rollText.Text, false);
				int potency = OnlyNumbers(potencyText.Text, false);
				double effect = (roll * (stat * (1 + (buff/100))) * (potency/10))/100;
				effect = Math.Ceiling(effect);
				if(roll != 10 && roll != 20)
					effect++;
				if(roll >= OnlyNumbers(crit.Text, false))
					effect *= 2;
				if(effect > 9999 && level < 30)
					effect = 9999;
				else if(effect > 99999 & level < 60)
					effect = 99999;
				else if(effect > 999999)
					effect = 999999;
				if(stat == 0)
					effect = 0;
				this.effect.Text = effect.ToString();
			}
		}
		private void roll1Button_Click(object sender,EventArgs e) { rollText.Text = "1"; }
		private void roll20Button_Click(object sender,EventArgs e) { rollText.Text = "20"; }
		private void EditRoll(int r)
		{
			int roll = 0;
			if(rollText.Text != "")
				roll = OnlyNumbers(rollText.Text, false);
			roll += r;
			if(roll > 20)
				roll = 20;
			else if(roll < 1)
				roll = 1;
			rollText.Text = roll.ToString();
		}
		private void EditPot(int p)
		{
			int potency = 0;
			if(potencyText.Text != "")
				potency = OnlyNumbers(potencyText.Text, false);
			potency += p;
			if(potency < 0)
				potency = 0;
			potencyText.Text = potency.ToString();
		}
		private void rollP1_Click(object sender,EventArgs e) {EditRoll(1);}
		private void rollP5_Click(object sender,EventArgs e) {EditRoll(5);}
		private void rollP10_Click(object sender,EventArgs e) {EditRoll(10);}
		private void rollM1_Click(object sender,EventArgs e) {EditRoll(-1);}
		private void rollM5_Click(object sender,EventArgs e) {EditRoll(-5);}
		private void rollM10_Click(object sender,EventArgs e) {EditRoll(-10);}

		private void potP10_Click(object sender,EventArgs e) {EditPot(10);}
		private void potP50_Click(object sender,EventArgs e) {EditPot(50);}
		private void potP100_Click(object sender,EventArgs e) {EditPot(100);}
		private void potM10_Click(object sender,EventArgs e) {EditPot(-10);}
		private void potM50_Click(object sender,EventArgs e) {EditPot(-50);}
		private void potM100_Click(object sender,EventArgs e) {EditPot(-100);}

		private void stat1Check_CheckedChanged(object sender,EventArgs e)
		{
			if(stat1Check.Checked)
			{
				stat2Check.Checked = false;
				statLabel.Text = stat1Check.Text;
				stat.Text = stat1Value.Text;
			}
			else if(!stat1Check.Checked)
			{
				stat2Check.Checked = true;
				statLabel.Text = stat2Check.Text;
				stat.Text = stat2Value.Text;
			}
			UpdateEffect(sender, e);
		}
		private void stat2Check_CheckedChanged(object sender,EventArgs e)
		{
			if(stat2Check.Checked)
			{
				stat1Check.Checked = false;
				statLabel.Text = stat2Check.Text;
				stat.Text = stat2Value.Text;
			}
			else if(!stat2Check.Checked)
			{
				stat1Check.Checked = true;
				statLabel.Text = stat1Check.Text;
				stat.Text = stat1Value.Text;
			}
			UpdateEffect(sender, e);
		}
#endregion

		#region Defense
		private void UpdateDamage(int d)
		{
			int damage = 0;
			if(this.damage.Text != "")
				damage = OnlyNumbers(this.damage.Text, false);
			damage += d;
			if(damage < 0)
				damage = 0;
			this.damage.Text = damage.ToString();
			UpdateFinalDamage();
		}
		private void UpdateFinalDamage()
		{
			double level = OnlyNumbers(defenseLevel.Text, false);
			double defense = OnlyNumbers(defenseStat.Text, false);
			double damage = OnlyNumbers(this.damage.Text, false);
			double resistance = OnlyNumbers(this.resistance.Text, true);
			double bas = 0, unit = 0, limit = 0, antigod = 0;
			if(tankCheck.Checked)
			{
				bas = 10;
				unit = 200;
				limit = .675;
				antigod = .95;
			}
			else if(casterCheck.Checked)
			{
				bas = 5;
				unit = 50;
				limit = .45;
				antigod = .45;
			}
			else if(meleeCheck.Checked)
			{
				bas = 10;
				unit = 100;
				limit = .55;
				antigod = .6;
			}
			else if(rangedCheck.Checked)
			{
				bas = 10;
				unit = 80;
				limit = .5;
				antigod = .55;
			}
			double stat = bas + Math.Floor(unit * Math.Pow(level, limit)) - (Math.Floor(unit * Math.Pow(level, limit)) % bas);
			double maxStat = bas + Math.Floor(unit * Math.Pow(90, limit)) - (Math.Floor(unit * Math.Pow(90, limit)) % bas);
			double chance = Math.Round(Math.Min(stat / maxStat, stat / maxStat * antigod), 4);
			double final = 0;
			if(resistance == 0)
				final = Math.Round((damage * (1 - chance) - stat) - defense);
			else
				final = Math.Round(((damage * (1 - (resistance/100))) * (1 - chance) - stat) - defense);
			if(final < 0)
				final = 0;
			finalDamage.Text = final.ToString();
		}
		private void damageP1_Click(object sender,EventArgs e) {UpdateDamage(1);}
		private void damageP5_Click(object sender,EventArgs e) {UpdateDamage(5);}
		private void damageP10_Click(object sender,EventArgs e) {UpdateDamage(10);}
		private void damageP100_Click(object sender,EventArgs e) {UpdateDamage(100);}
		private void damageP500_Click(object sender,EventArgs e) {UpdateDamage(500);}
		private void damageP1k_Click(object sender,EventArgs e) {UpdateDamage(1000);}
		private void damageM1_Click(object sender,EventArgs e) {UpdateDamage(-1);}
		private void damageM5_Click(object sender,EventArgs e) {UpdateDamage(-5);}
		private void damageM10_Click(object sender,EventArgs e) {UpdateDamage(-10);}
		private void damageM100_Click(object sender,EventArgs e) {UpdateDamage(-100);}
		private void damageM500_Click(object sender,EventArgs e) {UpdateDamage(-500);}
		private void damageM1k_Click(object sender,EventArgs e) {UpdateDamage(-1000);}

		private void defenseLevel_TextChanged(object sender,EventArgs e)
		{
			int def = 0;
			if(defenseLevel.Text == "") {
				defenseLevel.Text = "0";
				defenseLevel.SelectAll();
				def = OnlyNumbers(defenseLevel.Text, false);
			}
			if(def > 90)
				defenseLevel.Text = "0";
			UpdateFinalDamage();
		}
		private void defenseLevelDown_Click(object sender,EventArgs e)
		{
			int def = OnlyNumbers(defenseLevel.Text, false);
			def--;
			if(def < 0)
				def = 0;
			defenseLevel.Text = def.ToString();
		}
		private void defenseLevelUp_Click(object sender,EventArgs e)
		{
			int def = OnlyNumbers(defenseLevel.Text, false);
			def++;
			if(def > 90)
				def = 90;
			defenseLevel.Text = def.ToString();
		}
		private void resistance_TextChanged(object sender,EventArgs e)
		{
			if(resistance.Text == "") {
				resistance.Text = "0";
				resistance.SelectAll();
			}
			int res = OnlyNumbers(resistance.Text, true);
			if(res > 90)
				resistance.Text = "90";
			else if(res < -90)
				resistance.Text = "-90";

			UpdateFinalDamage();
		}
		private void resistanceDown_Click(object sender,EventArgs e)
		{
			int res = OnlyNumbers(resistance.Text, true);
			res -= 5;
			resistance.Text = res.ToString();
		}
		private void resistanceUp_Click(object sender,EventArgs e)
		{
			int res = OnlyNumbers(resistance.Text, true);
			res += 5;
			resistance.Text = res.ToString();
		}
		private void damage_TextChanged(object sender,EventArgs e)
		{
			if(damage.Text == "") {
				damage.Text = "0";
				damage.SelectAll();
			}
			UpdateFinalDamage();
		}
#endregion

		#region Spirit
		private void UpdateSpirit(int s)
		{
			int damage = 0;
			if(Sdamage.Text != "")
				damage = OnlyNumbers(Sdamage.Text, false);
			damage += s;
			if(damage < 0)
				damage = 0;
			Sdamage.Text = damage.ToString();
			UpdateFinalSpirit();
		}
		private void UpdateFinalSpirit()
		{
			double level = OnlyNumbers(spiritLevel.Text, false);
			double defense = OnlyNumbers(spiritStat.Text, false);
			double damage = OnlyNumbers(this.Sdamage.Text, false);
			double resistance = OnlyNumbers(this.Sresistance.Text, true);
			double bas = 0, unit = 0, limit = 0, antigod = 0;
			if(tankCheck.Checked)
			{
				bas = 10;
				unit = 200;
				limit = .675;
				antigod = .8;
			}
			else if(casterCheck.Checked)
			{
				bas = 5;
				unit = 50;
				limit = .45;
				antigod = .75;
			}
			else if(meleeCheck.Checked)
			{
				bas = 10;
				unit = 100;
				limit = .55;
				antigod = .45;
			}
			else if(rangedCheck.Checked)
			{
				bas = 10;
				unit = 80;
				limit = .5;
				antigod = .60;
			}
			double stat = bas + Math.Floor(unit * Math.Pow(level, limit)) - (Math.Floor(unit * Math.Pow(level, limit)) % bas);
			double maxStat = bas + Math.Floor(unit * Math.Pow(90, limit)) - (Math.Floor(unit * Math.Pow(90, limit)) % bas);
			double chance = Math.Round(Math.Min(stat / maxStat, stat / maxStat * antigod), 4);
			double final = 0;
			if(resistance == 0)
				final = Math.Round((damage * (1 - chance) - stat) - defense);
			else
				final = Math.Round(((damage * (1 - (resistance/100))) * (1 - chance) - stat) - defense);
			if(final < 0)
				final = 0;
			SfinalDamage.Text = final.ToString();
		}
		private void SdamageP1_Click(object sender,EventArgs e) {UpdateSpirit(1);}
		private void SdamageP5_Click(object sender,EventArgs e) {UpdateSpirit(5);}
		private void SdamageP10_Click(object sender,EventArgs e) {UpdateSpirit(10);}
		private void SdamageP100_Click(object sender,EventArgs e) {UpdateSpirit(100);}
		private void SdamageP500_Click(object sender,EventArgs e) {UpdateSpirit(500);}
		private void SdamageP1k_Click(object sender,EventArgs e) {UpdateSpirit(1000);}
		private void SdamageM1_Click(object sender,EventArgs e) {UpdateSpirit(-1);}
		private void SdamageM5_Click(object sender,EventArgs e) {UpdateSpirit(-5);}
		private void SdamageM10_Click(object sender,EventArgs e) {UpdateSpirit(-10);}
		private void SdamageM100_Click(object sender,EventArgs e) {UpdateSpirit(-100);}
		private void SdamageM500_Click(object sender,EventArgs e) {UpdateSpirit(-500);}
		private void SdamageM1k_Click(object sender,EventArgs e) {UpdateSpirit(-1000);}

		private void spiritLevel_TextChanged(object sender,EventArgs e)
		{
			int spirit = 0;
			if(spiritLevel.Text == "") {
				spiritLevel.Text = "0";
				spiritLevel.SelectAll();
				spirit = OnlyNumbers(spiritLevel.Text, false);
			}
			if(spirit > 90)
				spiritLevel.Text = "90";
			UpdateFinalSpirit();
		}
		private void spiritLevelDown_Click(object sender,EventArgs e)
		{
			int spirit = OnlyNumbers(spiritLevel.Text, false);
			spirit--;
			if(spirit < 0)
				spirit = 0;
			spiritLevel.Text = spirit.ToString();
		}
		private void spiritLevelUp_Click(object sender,EventArgs e)
		{
			int spirit = OnlyNumbers(spiritLevel.Text, false);
			spirit++;
			if(spirit > 90)
				spirit = 90;
			spiritLevel.Text = spirit.ToString();
		}
		private void Sresistance_TextChanged(object sender,EventArgs e)
		{
			if(Sresistance.Text == "") {
				Sresistance.Text = "0";
				Sresistance.SelectAll();
			}
			int res = OnlyNumbers(Sresistance.Text, true);
			if(res > 90)
				Sresistance.Text = "90";
			else if(res < -90)
				Sresistance.Text = "-90";

			UpdateFinalDamage();
		}
		private void SresistanceDown_Click(object sender,EventArgs e)
		{
			int res = OnlyNumbers(Sresistance.Text, true);
			res -= 5;
			Sresistance.Text = res.ToString();
		}
		private void SresistanceUp_Click(object sender,EventArgs e)
		{
			int res = OnlyNumbers(Sresistance.Text, true);
			res += 5;
			Sresistance.Text = res.ToString();
		}
		private void Sdamage_TextChanged(object sender,EventArgs e)
		{
			if(Sdamage.Text == "") {
				Sdamage.Text = "0";
				Sdamage.SelectAll();
			}
			UpdateFinalSpirit();
		}
		#endregion

		private void level_TextChanged(object sender,EventArgs e)
		{

		}

		List<TextBox> textBoxes = new List<TextBox>();
		List<CheckBox[]> checkBoxes = new List<CheckBox[]>();
		private void AddTracker()
		{
			TextBox skill = new TextBox()
			{
				Location = new Point(35, 330 + (cooldownTrackers * 30)),
				Size = new Size(126, 23),
				TextAlign = HorizontalAlignment.Center
			}; 
			textBoxes.Add(skill);
			Controls.Add(skill);
			CheckBox[] checks = new CheckBox[10];
			for(int i = 0; i < 10; i++)
			{
				checks[i] = new CheckBox()
				{
					Text = "",
					AutoSize = false,
					Size = new Size(15, 14),
					Location = new Point(190 + (i * 20), 335 + (cooldownTrackers * 30))
				};
				Controls.Add(checks[i]);
			}
			checkBoxes.Add(checks);
		}

		private void addTrackerButton_Click(object sender,EventArgs e)
		{
			cooldownTrackers++;
			AddTracker();
			ActiveForm.Size = new Size(450, 400 + (cooldownTrackers * 30));
		}

		private void removeTrackerButton_Click(object sender,EventArgs e)
		{
			if(cooldownTrackers > 0)
			{
				cooldownTrackers--;
				if(cooldownTrackers == 0)
					ActiveForm.Size = new Size(450, 365);
				else
					ActiveForm.Size = new Size(450, 400 + (cooldownTrackers * 30));
				Controls.Remove(textBoxes[cooldownTrackers]);
				textBoxes.RemoveAt(textBoxes.Count - 1);
				for(int i = 0; i < 10; i++)
				{
					Controls.Remove(checkBoxes[cooldownTrackers][i]);
				}
				checkBoxes.RemoveAt(checkBoxes.Count - 1);
			}
		}

		private void buffDown_Click(object sender,EventArgs e)
		{
			int buff = OnlyNumbers(this.buff.Text, true);
			buff -= 5;
			this.buff.Text = buff.ToString();
		}

		private void buffUp_Click(object sender,EventArgs e)
		{
			
			int buff = OnlyNumbers(this.buff.Text, true);
			buff += 5;
			this.buff.Text = buff.ToString();
		}
	}
}