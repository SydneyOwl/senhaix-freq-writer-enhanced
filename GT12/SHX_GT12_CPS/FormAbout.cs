using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using HID;

namespace SHX_GT12_CPS;

public partial class FormAbout : Form
{
    public FormAbout()
    {
        InitializeComponent();
        
        ver.Text = VERSION.Version;
        commit.Text = VERSION.GitCommitHash;
        ctime.Text = VERSION.BuildTime;
    }

    private void label6_Click(object sender, EventArgs e)
    {
        // throw new System.NotImplementedException();
    }

    private void label6_Click_1(object sender, EventArgs e)
    {
        // throw new System.NotImplementedException();
    }
}