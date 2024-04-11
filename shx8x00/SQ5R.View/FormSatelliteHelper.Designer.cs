using System.ComponentModel;

namespace SQ5R.View;

partial class FormSatelliteHelper
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.button1 = new System.Windows.Forms.Button();
        this.label1 = new System.Windows.Forms.Label();
        this.richTextBox1 = new System.Windows.Forms.RichTextBox();
        this.listBox1 = new System.Windows.Forms.ListBox();
        this.richTextBox2 = new System.Windows.Forms.RichTextBox();
        this.button2 = new System.Windows.Forms.Button();
        this.button3 = new System.Windows.Forms.Button();
        this.listBox2 = new System.Windows.Forms.ListBox();
        this.label2 = new System.Windows.Forms.Label();
        this.checkBox1 = new System.Windows.Forms.CheckBox();
        this.label3 = new System.Windows.Forms.Label();
        this.label4 = new System.Windows.Forms.Label();
        this.label5 = new System.Windows.Forms.Label();
        this.richTextBox3 = new System.Windows.Forms.RichTextBox();
        this.richTextBox4 = new System.Windows.Forms.RichTextBox();
        this.label6 = new System.Windows.Forms.Label();
        this.label7 = new System.Windows.Forms.Label();
        this.SuspendLayout();
        // 
        // button1
        // 
        this.button1.Location = new System.Drawing.Point(20, 433);
        this.button1.Margin = new System.Windows.Forms.Padding(2);
        this.button1.Name = "button1";
        this.button1.Size = new System.Drawing.Size(92, 31);
        this.button1.TabIndex = 0;
        this.button1.Text = "卫星数据更新";
        this.button1.UseVisualStyleBackColor = true;
        this.button1.Click += new System.EventHandler(this.button1_Click);
        // 
        // label1
        // 
        this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.label1.Location = new System.Drawing.Point(268, 85);
        this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(84, 29);
        this.label1.TabIndex = 4;
        this.label1.Text = "详情：";
        this.label1.Click += new System.EventHandler(this.label1_Click);
        // 
        // richTextBox1
        // 
        this.richTextBox1.Location = new System.Drawing.Point(268, 106);
        this.richTextBox1.Margin = new System.Windows.Forms.Padding(2);
        this.richTextBox1.Name = "richTextBox1";
        this.richTextBox1.ReadOnly = true;
        this.richTextBox1.Size = new System.Drawing.Size(294, 112);
        this.richTextBox1.TabIndex = 5;
        this.richTextBox1.Text = "";
        this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
        // 
        // listBox1
        // 
        this.listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.listBox1.FormattingEnabled = true;
        this.listBox1.ItemHeight = 20;
        this.listBox1.Location = new System.Drawing.Point(15, 85);
        this.listBox1.Margin = new System.Windows.Forms.Padding(2);
        this.listBox1.Name = "listBox1";
        this.listBox1.Size = new System.Drawing.Size(226, 324);
        this.listBox1.TabIndex = 6;
        this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
        // 
        // richTextBox2
        // 
        this.richTextBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.richTextBox2.Location = new System.Drawing.Point(52, 23);
        this.richTextBox2.Margin = new System.Windows.Forms.Padding(2);
        this.richTextBox2.Name = "richTextBox2";
        this.richTextBox2.Size = new System.Drawing.Size(189, 31);
        this.richTextBox2.TabIndex = 7;
        this.richTextBox2.Text = "";
        this.richTextBox2.TextChanged += new System.EventHandler(this.richTextBox2_TextChanged);
        // 
        // button2
        // 
        this.button2.Location = new System.Drawing.Point(483, 21);
        this.button2.Margin = new System.Windows.Forms.Padding(2);
        this.button2.Name = "button2";
        this.button2.Size = new System.Drawing.Size(62, 33);
        this.button2.TabIndex = 8;
        this.button2.Text = "搜索";
        this.button2.UseVisualStyleBackColor = true;
        // 
        // button3
        // 
        this.button3.Location = new System.Drawing.Point(469, 429);
        this.button3.Margin = new System.Windows.Forms.Padding(2);
        this.button3.Name = "button3";
        this.button3.Size = new System.Drawing.Size(92, 35);
        this.button3.TabIndex = 10;
        this.button3.Text = "插入信道";
        this.button3.UseVisualStyleBackColor = true;
        this.button3.Click += new System.EventHandler(this.button3_Click);
        // 
        // listBox2
        // 
        this.listBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.listBox2.FormattingEnabled = true;
        this.listBox2.ItemHeight = 20;
        this.listBox2.Location = new System.Drawing.Point(268, 249);
        this.listBox2.Name = "listBox2";
        this.listBox2.Size = new System.Drawing.Size(294, 144);
        this.listBox2.TabIndex = 11;
        this.listBox2.SelectedIndexChanged += new System.EventHandler(this.listBox2_SelectedIndexChanged);
        // 
        // label2
        // 
        this.label2.Location = new System.Drawing.Point(2, 31);
        this.label2.Name = "label2";
        this.label2.Size = new System.Drawing.Size(45, 31);
        this.label2.TabIndex = 12;
        this.label2.Text = "搜索：";
        this.label2.Click += new System.EventHandler(this.label2_Click);
        // 
        // checkBox1
        // 
        this.checkBox1.Checked = true;
        this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
        this.checkBox1.Location = new System.Drawing.Point(175, 434);
        this.checkBox1.Name = "checkBox1";
        this.checkBox1.Size = new System.Drawing.Size(64, 30);
        this.checkBox1.TabIndex = 13;
        this.checkBox1.Text = "多普勒";
        this.checkBox1.UseVisualStyleBackColor = true;
        this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
        // 
        // label3
        // 
        this.label3.Location = new System.Drawing.Point(245, 442);
        this.label3.Name = "label3";
        this.label3.Size = new System.Drawing.Size(48, 22);
        this.label3.TabIndex = 14;
        this.label3.Text = "步进:";
        // 
        // label4
        // 
        this.label4.Location = new System.Drawing.Point(385, 418);
        this.label4.Name = "label4";
        this.label4.Size = new System.Drawing.Size(57, 28);
        this.label4.TabIndex = 16;
        this.label4.Text = "kHz";
        this.label4.Click += new System.EventHandler(this.label4_Click);
        // 
        // label5
        // 
        this.label5.Location = new System.Drawing.Point(280, 419);
        this.label5.Name = "label5";
        this.label5.Size = new System.Drawing.Size(36, 13);
        this.label5.TabIndex = 17;
        this.label5.Text = "U段±";
        // 
        // richTextBox3
        // 
        this.richTextBox3.Location = new System.Drawing.Point(315, 414);
        this.richTextBox3.Name = "richTextBox3";
        this.richTextBox3.Size = new System.Drawing.Size(64, 27);
        this.richTextBox3.TabIndex = 18;
        this.richTextBox3.Text = "10";
        // 
        // richTextBox4
        // 
        this.richTextBox4.Location = new System.Drawing.Point(315, 448);
        this.richTextBox4.Name = "richTextBox4";
        this.richTextBox4.Size = new System.Drawing.Size(64, 27);
        this.richTextBox4.TabIndex = 21;
        this.richTextBox4.Text = "5";
        // 
        // label6
        // 
        this.label6.Location = new System.Drawing.Point(280, 451);
        this.label6.Name = "label6";
        this.label6.Size = new System.Drawing.Size(36, 13);
        this.label6.TabIndex = 20;
        this.label6.Text = "V段±";
        // 
        // label7
        // 
        this.label7.Location = new System.Drawing.Point(385, 451);
        this.label7.Name = "label7";
        this.label7.Size = new System.Drawing.Size(57, 28);
        this.label7.TabIndex = 19;
        this.label7.Text = "kHz";
        this.label7.Click += new System.EventHandler(this.label7_Click);
        // 
        // FromSatelliteHelper
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.BackColor = System.Drawing.SystemColors.Control;
        this.ClientSize = new System.Drawing.Size(572, 486);
        this.Controls.Add(this.richTextBox4);
        this.Controls.Add(this.label6);
        this.Controls.Add(this.label7);
        this.Controls.Add(this.richTextBox3);
        this.Controls.Add(this.label5);
        this.Controls.Add(this.label4);
        this.Controls.Add(this.label3);
        this.Controls.Add(this.checkBox1);
        this.Controls.Add(this.label2);
        this.Controls.Add(this.listBox2);
        this.Controls.Add(this.button3);
        this.Controls.Add(this.button2);
        this.Controls.Add(this.richTextBox2);
        this.Controls.Add(this.listBox1);
        this.Controls.Add(this.richTextBox1);
        this.Controls.Add(this.label1);
        this.Controls.Add(this.button1);
        this.Location = new System.Drawing.Point(15, 15);
        this.Margin = new System.Windows.Forms.Padding(2);
        this.Name = "FormSatelliteHelper";
        this.Text = "打星助手";
        this.Load += new System.EventHandler(this.FromSatelliteHelper_Load);
        this.ResumeLayout(false);
    }

    private System.Windows.Forms.RichTextBox richTextBox4;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label7;

    private System.Windows.Forms.Label label5;

    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.RichTextBox richTextBox3;
    private System.Windows.Forms.Label label4;

    private System.Windows.Forms.CheckBox checkBox1;

    private System.Windows.Forms.Label label2;

    private System.Windows.Forms.ListBox listBox2;

    private System.Windows.Forms.Button button3;

    private System.Windows.Forms.Button button2;

    private System.Windows.Forms.RichTextBox richTextBox2;

    private System.Windows.Forms.ListBox listBox1;

    private System.Windows.Forms.RichTextBox richTextBox1;

    private System.Windows.Forms.Label label1;

    private System.Windows.Forms.Button button1;

    #endregion
}