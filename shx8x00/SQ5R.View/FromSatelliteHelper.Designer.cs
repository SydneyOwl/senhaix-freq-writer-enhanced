using System.ComponentModel;

namespace SQ5R.View;

partial class FromSatelliteHelper
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
        this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
        this.button3 = new System.Windows.Forms.Button();
        this.SuspendLayout();
        // 
        // button1
        // 
        this.button1.Location = new System.Drawing.Point(39, 832);
        this.button1.Name = "button1";
        this.button1.Size = new System.Drawing.Size(157, 60);
        this.button1.TabIndex = 0;
        this.button1.Text = "数据更新";
        this.button1.UseVisualStyleBackColor = true;
        this.button1.Click += new System.EventHandler(this.button1_Click);
        // 
        // label1
        // 
        this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.label1.Location = new System.Drawing.Point(537, 145);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(169, 56);
        this.label1.TabIndex = 4;
        this.label1.Text = "详情：";
        this.label1.Click += new System.EventHandler(this.label1_Click);
        // 
        // richTextBox1
        // 
        this.richTextBox1.Location = new System.Drawing.Point(537, 204);
        this.richTextBox1.Name = "richTextBox1";
        this.richTextBox1.Size = new System.Drawing.Size(585, 181);
        this.richTextBox1.TabIndex = 5;
        this.richTextBox1.Text = "";
        // 
        // listBox1
        // 
        this.listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.listBox1.FormattingEnabled = true;
        this.listBox1.ItemHeight = 37;
        this.listBox1.Location = new System.Drawing.Point(30, 164);
        this.listBox1.Name = "listBox1";
        this.listBox1.Size = new System.Drawing.Size(449, 633);
        this.listBox1.TabIndex = 6;
        this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
        // 
        // richTextBox2
        // 
        this.richTextBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.richTextBox2.Location = new System.Drawing.Point(30, 44);
        this.richTextBox2.Name = "richTextBox2";
        this.richTextBox2.Size = new System.Drawing.Size(449, 68);
        this.richTextBox2.TabIndex = 7;
        this.richTextBox2.Text = "";
        // 
        // button2
        // 
        this.button2.Location = new System.Drawing.Point(537, 44);
        this.button2.Name = "button2";
        this.button2.Size = new System.Drawing.Size(125, 64);
        this.button2.TabIndex = 8;
        this.button2.Text = "搜索";
        this.button2.UseVisualStyleBackColor = true;
        // 
        // checkedListBox1
        // 
        this.checkedListBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.checkedListBox1.FormattingEnabled = true;
        this.checkedListBox1.Location = new System.Drawing.Point(540, 427);
        this.checkedListBox1.Name = "checkedListBox1";
        this.checkedListBox1.Size = new System.Drawing.Size(581, 355);
        this.checkedListBox1.TabIndex = 9;
        this.checkedListBox1.SelectedIndexChanged += new System.EventHandler(this.checkedListBox1_SelectedIndexChanged);
        // 
        // button3
        // 
        this.button3.Location = new System.Drawing.Point(938, 825);
        this.button3.Name = "button3";
        this.button3.Size = new System.Drawing.Size(183, 67);
        this.button3.TabIndex = 10;
        this.button3.Text = "插入信道";
        this.button3.UseVisualStyleBackColor = true;
        // 
        // FromSatelliteHelper
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.BackColor = System.Drawing.SystemColors.Control;
        this.ClientSize = new System.Drawing.Size(1144, 935);
        this.Controls.Add(this.button3);
        this.Controls.Add(this.checkedListBox1);
        this.Controls.Add(this.button2);
        this.Controls.Add(this.richTextBox2);
        this.Controls.Add(this.listBox1);
        this.Controls.Add(this.richTextBox1);
        this.Controls.Add(this.label1);
        this.Controls.Add(this.button1);
        this.Location = new System.Drawing.Point(15, 15);
        this.Name = "FromSatelliteHelper";
        this.Load += new System.EventHandler(this.FromSatelliteHelper_Load);
        this.ResumeLayout(false);
    }

    private System.Windows.Forms.Button button3;

    private System.Windows.Forms.CheckedListBox checkedListBox1;

    private System.Windows.Forms.Button button2;

    private System.Windows.Forms.RichTextBox richTextBox2;

    private System.Windows.Forms.ListBox listBox1;

    private System.Windows.Forms.RichTextBox richTextBox1;

    private System.Windows.Forms.Label label1;

    private System.Windows.Forms.Button button1;

    #endregion
}