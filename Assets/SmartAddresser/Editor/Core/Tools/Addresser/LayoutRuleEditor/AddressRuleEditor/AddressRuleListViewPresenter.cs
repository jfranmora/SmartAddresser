using System;
using System.Collections.Generic;
using SmartAddresser.Editor.Core.Models.LayoutRules.AddressRules;
using SmartAddresser.Editor.Core.Tools.Addresser.Shared;
using SmartAddresser.Editor.Foundation.CommandBasedUndo;
using SmartAddresser.Editor.Foundation.TinyRx;
using SmartAddresser.Editor.Foundation.TinyRx.ObservableCollection;

namespace SmartAddresser.Editor.Core.Tools.Addresser.LayoutRuleEditor.AddressRuleEditor
{
    /// <summary>
    ///     Presenter for <see cref="AddressRuleEditorListView" />.
    /// </summary>
    internal sealed class AddressRuleListViewPresenter : IDisposable
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private readonly Dictionary<string, AddressRuleListTreeView.Item> _ruleIdToTreeViewItem =
            new Dictionary<string, AddressRuleListTreeView.Item>();

        private readonly AddressRuleEditorListView _view;

        public AddressRuleListViewPresenter(AddressRuleEditorListView view, AutoIncrementHistory history,
            IAssetSaveService saveService)
        {
            _view = view;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        public void SetupView(IObservableList<AddressRule> rules)
        {
            CleanupView();

            rules.ObservableAdd.Subscribe(x => AddRuleView(x.Value, x.Index)).DisposeWith(_disposables);
            rules.ObservableRemove.Subscribe(x => RemoveRuleView(x.Value)).DisposeWith(_disposables);
            rules.ObservableClear.Subscribe(_ => ClearViews()).DisposeWith(_disposables);
            foreach (var rule in rules)
                AddRuleView(rule);
            _view.TreeView.Reload();

            #region Local methods

            void AddRuleView(AddressRule rule, int index = -1, bool reload = true)
            {
                var item = _view.TreeView.AddItem(rule, index);
                _ruleIdToTreeViewItem.Add(rule.Id, item);
                if (reload)
                    _view.TreeView.Reload();
            }

            void RemoveRuleView(AddressRule rule)
            {
                var item = _ruleIdToTreeViewItem[rule.Id];
                _ruleIdToTreeViewItem.Remove(rule.Id);
                _view.TreeView.RemoveItem(item.id);
                _view.TreeView.Reload();
            }

            void ClearViews()
            {
                _view.TreeView.ClearItems();
                _view.TreeView.Reload();
                _ruleIdToTreeViewItem.Clear();
            }

            #endregion
        }

        public void CleanupView()
        {
            _view.TreeView.ClearItems();
            _view.TreeView.Reload();
        }
    }
}