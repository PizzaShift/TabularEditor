﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.BestPracticeAnalyzer;
using TabularEditor.TOMWrapper;
using System.Linq.Dynamic;
using Aga.Controls.Tree;
using System.Collections;
using System.Threading;

namespace TabularEditor.UI.Dialogs
{
    public partial class BPAForm : Form
    {
        ListViewGroup lvgLocal;
        ListViewGroup lvgGlobal;

        Analyzer analyzer;

        public Dictionary<string, BestPracticeRule> RuleIndex = new Dictionary<string, BestPracticeRule>();

        public Model Model { get { return analyzer.Model; } set { SetModel(value); } }

        private void SetModel(Model model)
        {
            if (model != analyzer.Model)
            {
                AnalyzerResultsTreeModel.Clear();
                btnRefresh.Enabled = model != null;
                analyzer.Model = model;
                toolStripStatusLabel1.Text = "";
            }
        }

        public BPAForm()
        {
            InitializeComponent();

            analyzer = new Analyzer();

            btnRefresh.Enabled = Model != null;

            tvResults.Model = AnalyzerResultsTreeModel;
            AnalyzerResultsTreeModel.StructureChanged += AnalyzerResultsTreeModel_StructureChanged;
            tvResults.DefaultToolTipProvider = new AnalyzerResultTooltip();
        }

        private void AnalyzerResultsTreeModel_StructureChanged(object sender, TreePathEventArgs e)
        {
            AutofitColObject();
        }

        BPAEditorForm editor = new BPAEditorForm();

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            using (var hourglass = new Hourglass())
            {
                btnRefresh.Enabled = false;
                toolStripStatusLabel1.Text = "Analyzing...";
                Application.DoEvents();
                AnalyzeAll();
                btnRefresh.Enabled = true;
            }
        }

        public void Goto(AnalyzerResult result)
        {
            var obj = result.Object as ITabularNamedObject;
            UIController.Current.Goto(obj);
        }

        internal AnalyzerResultsModel AnalyzerResultsTreeModel { get; private set; } = new AnalyzerResultsModel();

        public void RefreshUI()
        {
            var oC = AnalyzerResultsTreeModel.ObjectCount;
            var rC = AnalyzerResultsTreeModel.RuleCount;
            toolStripStatusLabel1.Text = string.Format("{0} object{1} in violation of {2} Best Practice rule{3}.",
                oC, oC == 1 ? "" : "s", rC, rC == 1 ? "" : "s");
        }

        public void Analyze(IEnumerable<BestPracticeRule> rules)
        {
            AnalyzerResultsTreeModel.Update(analyzer.Analyze(rules));
            RefreshUI();
        }

        public void AnalyzeAll()
        {
            Analyze(analyzer.GlobalRules.Concat(analyzer.LocalRules));
        }

        /// <summary>
        /// Analyzes all rules.
        /// </summary>
        /// <param name="token"></param>
        public void AnalyzeAll(CancellationToken token)
        {
            AnalyzerResultsTreeModel.Update(analyzer.AnalyzeAll(token));
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

            if (tvResults.SelectedNodes.Count == 0) {
                e.Cancel = true;
                return;
            }
            var plural = tvResults.SelectedNodes.Count > 1;


            //// SubItems[2] contains the ID of the respective rule:
            //var rules = tvResults.SelectedItems.Cast<ListViewItem>().Select(i => i.SubItems[2].Text).Distinct().Select(n => RuleIndex[n]).ToList();

            //bpaResultGoTo.Visible = !plural;
            //bpaResultGoToSep.Visible = !plural;

            //var p = "Selected object" + (plural ? "s" : "");
            //bpaResultIgnoreRule.Enabled = rules.Count == 1;
            //bpaResultIgnoreSelected.Text = p;
            //bpaResultScriptSelected.Text = p;
            //bpaResultFixSelected.Text = p;

            //var canFix = rules.Any(r => !string.IsNullOrEmpty(r.FixExpression));
            //bpaResultScript.Enabled = canFix;
            //bpaResultFix.Enabled = canFix;
        }

        private void bpaResultGoTo_Click(object sender, EventArgs e)
        {
            //if(tvResults.SelectedItems.Count == 1)
            //{
            //    Goto(tvResults.SelectedItems[0]);
            //}
        }

        private void bpaResultIgnoreSelected_Click(object sender, EventArgs e)
        {
            //bool unsupported = false;

            //foreach (ListViewItem item in tvResults.SelectedItems)
            //{
            //    var rule = RuleIndex[item.SubItems[3].Text];
            //    var obj = item.Tag as IAnnotationObject;

            //    if (obj == null) unsupported = true;
            //    else analyzer.IgnoreRule(rule, true, obj);
            //}

            //if (unsupported)
            //{
            //    MessageBox.Show("One or more of the selected objects does not support annotations. For this reason, the rule cannot be ignored on these objects.", "Cannot ignore rule", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
        }

        private void bpaResultIgnoreRule_Click(object sender, EventArgs e)
        {
            //var rules = tvResults.SelectedItems.Cast<ListViewItem>().Select(i => i.SubItems[3].Text).Distinct().Select(n => RuleIndex[n]).ToList();

            //foreach (var rule in rules)
            //{
            //    analyzer.IgnoreRule(rule);
            //    listView1.Items[rule.ID].Checked = false;
            //}
        }

        private void bpaResultScriptSelected_Click(object sender, EventArgs e)
        {
            //var script = string.Join("\n", tvResults.SelectedItems.Cast<ListViewItem>().Select(
            //    i =>
            //    {
            //        var obj = i.Tag as TabularNamedObject;
            //        var rule = RuleIndex[i.SubItems[3].Text];
            //        if (string.IsNullOrEmpty(rule.FixExpression)) return string.Format("// No automatic fix for rule '{0}' on object {1}", i.SubItems[2], i.SubItems[0]);
            //        return obj.GetLinqPath() + "." + rule.FixExpression + ";";
            //    }
            //    ).ToArray());

            //Clipboard.SetText(script);
            //MessageBox.Show("Fix script copied to clipboard!\n\nPaste into Advanced Script Editor for review.", "Fix script generation", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /*
                private void btnAdd_Click(object sender, EventArgs e)
                {
                    editor.PopulateCategories(analyzer.AllRules);
                    var newRule = editor.NewRule(analyzer.GetUniqueId("New Rule"));
                    if (newRule != null)
                    {
                        analyzer.AddRule(newRule);
                        analyzer.SaveLocalRulesToModel();
                        UIController.Current.InvokeBPABackground();
                    }
                }

                private void btnEdit_Click(object sender, EventArgs e)
                {
                    if (listView1.SelectedItems.Count == 1)
                    {
                        var rule = listView1.SelectedItems[0].Tag as BestPracticeRule;
                        editor.PopulateCategories(analyzer.AllRules);
                        var oldRuleId = rule.ID;
                        if(editor.EditRule(rule))
                        {
                            if(analyzer.LocalRules.Contains(rule))
                            {
                                analyzer.SaveLocalRulesToModel();
                            } else if(analyzer.GlobalRules.Contains(rule))
                            {
                                var bpc = new BestPracticeCollection();
                                bpc.Add(rule);

                                var globalRulesFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\TabularEditor\BPARules.json";

                                bpc.AddFromJsonFile(globalRulesFile);
                                if (oldRuleId != rule.ID) {
                                    // ID changed - let's delete the rule with the old ID:
                                    var oldRule = bpc.FirstOrDefault(r => r.ID == oldRuleId);
                                    if (oldRule != null) bpc.Remove(oldRule);
                                }
                                bpc.SaveToFile(globalRulesFile);
                            }

                            PopulateListView();
                            UIController.Current.InvokeBPABackground();
                        }
                    }
                }
        */

        private void BPAForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Hide the form instead of closing it:
            Hide();
            e.Cancel = true;
        }

        /*
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count == 1 && listView1.SelectedItems[0].Group == lvgLocal)
            {
                var item = listView1.SelectedItems[0];
                var res = MessageBox.Show("Are you sure you want to delete this rule from the model?", "Delete rule?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (res == DialogResult.Cancel) return;

                analyzer.LocalRules.Remove(item.Tag as BestPracticeRule);
                analyzer.SaveLocalRulesToModel();
                item.Remove();
            }
        }

        private void LocalToGlobal()
        {
            // Convert a rule from local to global:
            var item = listView1.SelectedItems[0];
            var rule = item.Tag as BestPracticeRule;
            analyzer.LocalRules.Remove(rule);
            analyzer.GlobalRules.Add(rule);
            item.Group = lvgGlobal;

            analyzer.SaveLocalRulesToModel();

            // Save global rules (adding the newly promoted rule):
            var bpc = new BestPracticeCollection();
            bpc.Add(rule);

            var globalRulesFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\TabularEditor\BPARules.json";

            bpc.AddFromJsonFile(globalRulesFile);
            bpc.SaveToFile(globalRulesFile);
        }

        private void GlobalToLocal()
        {
            // Convert a rule from global to local:
            var item = listView1.SelectedItems[0];
            var rule = item.Tag as BestPracticeRule;
            analyzer.GlobalRules.Remove(rule);
            analyzer.LocalRules.Add(rule);
            item.Group = lvgLocal;

            analyzer.SaveLocalRulesToModel();

            // Save global rules (less the newly demoted rule):
            var bpc = new BestPracticeCollection();
            var globalRulesFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\TabularEditor\BPARules.json";
            bpc.AddFromJsonFile(globalRulesFile);
            var deleteRule = bpc.FirstOrDefault(r => r.ID.Equals(rule.ID, StringComparison.InvariantCultureIgnoreCase));
            if (deleteRule != null)
            {
                bpc.Remove(deleteRule);
                bpc.SaveToFile(globalRulesFile);
            }
        }*/

        private void bpaResultScriptRule_Click(object sender, EventArgs e)
        {
            //if(tvResults.SelectedItems.Count == 1)
            //{
            //    var item = tvResults.SelectedItems[0];
            //    var rule = RuleIndex[item.SubItems[3].Text];

            //    if (string.IsNullOrEmpty(rule.FixExpression))
            //    {
            //        MessageBox.Show("No automatic fix exists on this rule.", "No automatic fix", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        return;
            //    }

            //    var script = string.Join("\n", analyzer.Analyze(rule).Select(
            //        ar =>
            //        {
            //            var obj = ar.Object;
            //            return obj.GetLinqPath() + "." + rule.FixExpression + ";";
            //        }
            //        ).ToArray());

            //    Clipboard.SetText(script);
            //    MessageBox.Show("Fix script copied to clipboard!\n\nPaste into Advanced Script Editor for review.", "Fix script generation", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //}
        }

        private void txtObjectName_ValueNeeded(object sender, Aga.Controls.Tree.NodeControls.NodeControlValueEventArgs e)
        {
            if (e.Node.Tag is BestPracticeRule rule)
            {
                var objCount = AnalyzerResultsTreeModel.ObjectCountByRule(rule);
                e.Value = rule.Name + " (" + objCount + " object" + (objCount == 1 ? "" : "s") + ")";
            }
            else if (e.Node.Tag is AnalyzerResult result)
            {
                e.Value = result.ObjectName;
            }
        }

        public void ShowBPA()
        {
            Show();
            BringToFront();
            RefreshUI();
        }

        private void txtObjectName_DrawText(object sender, Aga.Controls.Tree.NodeControls.DrawEventArgs e)
        {
            if (e.Node.Tag is BestPracticeRule rule)
            {
                e.Font = new Font(e.Font, FontStyle.Bold);
                if (!rule.Enabled) e.TextColor = e.Context.DrawSelection == DrawSelectionMode.None ? SystemColors.GrayText : Color.Silver;
                if (e.Control == txtObjectName)
                    e.FullRowDraw = true;
                else if (e.Control == txtObjectType)
                    e.SkipDraw = true;
            } else if (e.Node.Tag is AnalyzerResult result)
            {
                if(result.Ignored) e.TextColor = e.Context.DrawSelection == DrawSelectionMode.None ? SystemColors.GrayText : Color.Silver;
            }
        }

        private void txtObjectType_DrawText(object sender, Aga.Controls.Tree.NodeControls.DrawEventArgs e)
        {
            if (e.Node.Tag is AnalyzerResult result)
            {
                if (result.Ignored) e.TextColor = e.Context.DrawSelection == DrawSelectionMode.None ? SystemColors.GrayText : Color.Silver;
            }
        }

        #region Auto-sizing tree view columns
        bool treeViewResizing = false;

        private void tvResults_Resize(object sender, EventArgs e)
        {
            AutofitColObject();
        }

        private void colObject_WidthChanged(object sender, EventArgs e)
        {
            if (!treeViewResizing)
            {
                colType.MinColumnWidth = 0;
                colType.MaxColumnWidth = 0;
                colType.Width = tvResults.ClientRectangle.Width - colObject.Width -
                    (tvResults.VerticalScrollbarVisible ? SystemInformation.VerticalScrollBarWidth : 0);
                colType.MinColumnWidth = colType.Width;
                colType.MaxColumnWidth = colType.Width;
            }
        }

        private void AutofitColObject()
        {
            treeViewResizing = true;
            colObject.Width = tvResults.ClientRectangle.Width - colType.Width -
                (tvResults.VerticalScrollbarVisible ? SystemInformation.VerticalScrollBarWidth : 0);
            treeViewResizing = false;
        }

        private void tvResults_Expanded(object sender, TreeViewAdvEventArgs e)
        {
            AutofitColObject();
        }

        private void tvResults_Collapsed(object sender, TreeViewAdvEventArgs e)
        {
            AutofitColObject();
        }
        #endregion

        private void tvResults_NodeMouseDoubleClick(object sender, TreeNodeAdvMouseEventArgs e)
        {
            if (e.Node.Tag is AnalyzerResult result)
            {
                Goto(result);
            }
        }

        private void btnGoto_Click(object sender, EventArgs e)
        {
            if (Selection.Count == 1)
                Goto(Selection[0]);
        }

        private void tvResults_SelectionChanged(object sender, EventArgs e)
        {
            Selection.Clear();
            foreach(var node in tvResults.SelectedNodes)
            {
                if(node.Tag is BestPracticeRule rule)
                {
                    Selection.AddRange(AnalyzerResultsTreeModel.ResultsByRule(rule));
                } else if(node.Tag is AnalyzerResult result)
                {
                    Selection.Add(result);
                }
            }
            BeginInvoke(new Action(UpdateUI));
        }

        private List<AnalyzerResult> Selection = new List<AnalyzerResult>();

        private bool CanGotoSelection => Selection.Count == 1;
        private bool CanFixSelection => Selection.Count >= 1 && Selection.All(r => r.CanFix) && Selection.Any(r => !r.Ignored);
        private bool CanIgnoreSelection =>
            tvResults.SelectedNodes.Count > 0 &&
            RuleSelection ?
                tvResults.SelectedNodes.Any(n => (n.Tag as BestPracticeRule).Enabled) :
                tvResults.SelectedNodes.Any(n => !(n.Tag as AnalyzerResult).Ignored);
        private bool CanUnignoreSelection =>
            tvResults.SelectedNodes.Count > 0 &&
            RuleSelection ?
                tvResults.SelectedNodes.Any(n => !(n.Tag as BestPracticeRule).Enabled) :
                tvResults.SelectedNodes.Any(n => (n.Tag as AnalyzerResult).Ignored);

        private bool RuleSelection => tvResults.SelectedNodes.Count > 0 && tvResults.SelectedNodes[0].Tag is BestPracticeRule;

        private void UpdateUI()
        {
            // Goto-button requires a single selection:
            btnGoto.Enabled = CanGotoSelection;

            // Script-button and Fix-button requires at least a single selection, and that all objects in the selection can be fixed:
            btnScript.Enabled = CanFixSelection;
            btnFix.Enabled = CanFixSelection;

            // Ignore-button requires at least a single selection:
            btnIgnore.Enabled = CanIgnoreSelection || CanUnignoreSelection;
            btnIgnore.CheckState = CanIgnoreSelection ?
                (CanUnignoreSelection ? CheckState.Indeterminate : CheckState.Unchecked) :
                (CanUnignoreSelection ? CheckState.Checked : CheckState.Indeterminate);

            if (tvResults.SelectedNodes.Count > 0)
            {
                if (tvResults.SelectedNodes[0].Tag is BestPracticeRule rule)
                {
                    btnIgnore.Text = (CanIgnoreSelection ? "Ignore" : "Unignore") + " selected rule" + (tvResults.SelectedNodes.Count == 1 ? "" : "s");
                }
                else if (tvResults.SelectedNodes[0].Tag is AnalyzerResult result)
                    btnIgnore.Text = (CanIgnoreSelection ? "Ignore" : "Unignore") + $" rule '{result.RuleName}' on selected item" + (tvResults.SelectedNodes.Count == 1 ? "" : "s");
            }
            else
                btnIgnore.Text = (CanIgnoreSelection ? "Ignore" : "Unignore") + " selection";
        }

        private void btnIgnore_Click(object sender, EventArgs e)
        {
            bool unsupported = false;

            UIController.Current.Handler.BeginUpdate("Ignore BPA rules");

            if (RuleSelection)
            {
                // Selected rules:
                foreach (var rule in tvResults.SelectedNodes.Select(n => n.Tag).OfType<BestPracticeRule>())
                {
                    analyzer.IgnoreRule(rule, btnIgnore.CheckState == CheckState.Checked);
                }

            }
            else
            {
                // Selected individual items:
                foreach (var node in tvResults.SelectedNodes.Select(n => n.Tag).OfType<AnalyzerResult>())
                {
                    var rule = node.Rule;

                    if (!(node.Object is IAnnotationObject obj)) unsupported = true;
                    else analyzer.IgnoreRule(rule, btnIgnore.CheckState == CheckState.Checked, obj);
                }
            }
            UIController.Current.Handler.EndUpdate();

            if (unsupported)
            {
                MessageBox.Show("One or more of the selected objects does not support annotations. For this reason, the rule cannot be ignored on these objects.", "Cannot ignore rule", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private string GetFixScript()
        {
            return GetFixScript(Selection.Where(r => r.CanFix && !r.Ignored));
        }

        private string GetFixScript(IEnumerable<AnalyzerResult> items)
        {
            string script = "";

            foreach (var item in items)
            {
                if (script != "") script += "\n";
                script += item.Object.GetLinqPath() + "." + item.Rule.FixExpression + ";";
            }

            return script;
        }

        private void btnScript_Click(object sender, EventArgs e)
        {
            var script = GetFixScript();

            Clipboard.SetText(script);
            MessageBox.Show("Fix script copied to clipboard!\n\nPaste into Advanced Script Editor for review.", "Fix script generation", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnFix_Click(object sender, EventArgs e)
        {
            var script = GetFixScript();

            System.CodeDom.Compiler.CompilerResults result;
            Scripting.ScriptOutputForm.Reset(false);
            var dyn = ScriptEngine.CompileScript(script, out result);
            if (result.Errors.Count > 0)
            {
                MessageBox.Show("Could not apply fix automatically. Use the 'Generate fix script' option instead.", "Apply fix", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                UIController.Current.Handler.BeginUpdate("apply BPA fix");
                dyn.Invoke(Model, null);
                UIController.Current.Handler.EndUpdateAll();
            }
            catch (Exception)
            {
                UIController.Current.Handler.EndUpdateAll(true);
                MessageBox.Show("Could not apply fix automatically. Use the 'Generate fix script' option instead.", "Apply fix", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnShowIgnored_Click(object sender, EventArgs e)
        {
            AnalyzerResultsTreeModel.ShowIgnored = btnShowIgnored.Checked;
        }
    }
}
