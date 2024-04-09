using System;
using System.Windows.Forms;
using SQ5R.Properties;

namespace SQ5R.View;

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