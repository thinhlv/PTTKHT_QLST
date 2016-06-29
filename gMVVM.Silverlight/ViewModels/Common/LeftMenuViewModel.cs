using gMVVM.gMVVMService;
using gMVVM.CommonClass;
using gMVVM.Resources;
using mvvmCommon;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.Net;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace gMVVM.ViewModels.Common
{
    public class LeftMenuViewModel : ViewModelBase
    {
        private bool isVN = true;
        public LeftMenuViewModel()
        {
            if (!IsolatedStorageSettings.ApplicationSettings["currentCulture"].ToString().Equals("vi-VN"))
                this.isVN = false;

            TLMENUClient menuClient = new TLMENUClient();
            menuClient.GetByTopTLMENUAsync("", "", " MENU_PARENT, MENU_ORDER ");

            menuClient.GetByTopTLMENUCompleted += new EventHandler<GetByTopTLMENUCompletedEventArgs>(getMenuCompleted);           
        }

        private void getMenuCompleted(object sender, GetByTopTLMENUCompletedEventArgs e)
        {
            if (e.Result != null && e.Result.Count > 0)
            {
                this.lstHyperlink = new List<HyperlinkButton>();
                Style hyperlinkStyle = Application.Current.Resources["HyperlinkButtonMenuitemStyle"] as Style;
                Style accordionItemStyle = Application.Current.Resources["AccordionItemParentStyle"] as Style;
                Style hplParent = Application.Current.Resources["HyperlinkButtonMenuitemStyle"] as Style;
                Style accordionItemParent = Application.Current.Resources["AccordionItemHyperlinkStyle"] as Style;

                //Style hyperlinkStyle = Application.Current.Resources["LinkStyle"] as Style;
                //Style accordionItemStyle = Application.Current.Resources["gAccordionButtonStyle1"] as Style;
                //Style hplParent = Application.Current.Resources["HplParent"] as Style;
                //Style accordionItemParent = Application.Current.Resources["gAccordionParent"] as Style;
                List<string> lstTempIamge = new List<string>() { "/gMVVM;component/Data/Icons/He Thong.png","/gMVVM;component/Data/Icons/He Thong.png", "/gMVVM;component/Data/Icons/Danh muc.png"
                , "/gMVVM;component/Data/Icons/Ke hoach.png", "/gMVVM;component/Data/Icons/Mua sam.png"
                , "/gMVVM;component/Data/Icons/Tai san.png", "/gMVVM;component/Data/Icons/Xe.png", "/gMVVM;component/Data/Icons/CT_XDCB.png"
                , "/gMVVM;component/Data/Icons/CTXDCB.png", "/gMVVM;component/Data/Icons/Bao cao.png"};
                ObservableCollection<TL_MENU> lstMenu = e.Result;
                List<AccordionItem> lstParentMenu = new List<AccordionItem>();//List Parent Menu
                AccordionItem accParentMenu;//Parent Menu
                StackPanel spnChild;//List Menu Child
                HyperlinkButton hlkChild;//Menu child                
                Dictionary<string, AccordionItem> dicAccParent = new Dictionary<string, AccordionItem>();
                string menuNname = "";
                CurrentSystemInfor.AvailableLink = new Dictionary<string, string>();                
                int i=0;
                foreach (var item in lstMenu)
                {
                    if (CurrentSystemLogin.Roles.ContainsKey(item.MENU_ID.ToString()))
                    {
                        menuNname = this.isVN ? item.MENU_NAME : item.MENU_NAME_EL;
                        if (item.MENU_PARENT.Equals(""))
                        {
                            if (!item.MENU_LINK.Equals("/"))
                            {
                                accParentMenu = GetAccordionItemHyperlink(menuNname, item.MENU_ID, item.MENU_LINK, lstTempIamge[i%2], accordionItemParent);
                                accParentMenu.ApplyTemplate();
                                this.lstHyperlink.Add(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(accParentMenu, 0) as Grid, 0) as HyperlinkButton);
                                //accParentMenu = GetAccordionItemParent(GetHyperLinkParent(item.MENU_ID, menuNname, item.MENU_LINK, hplParent), accordionItemParent);
                                lstParentMenu.Add(accParentMenu);
                            }
                            else
                            {
                                accParentMenu = GetAccordionItem(menuNname, accordionItemStyle, lstTempIamge[i % 10]);
                                spnChild = CreateStackPanel();
                                accParentMenu.Content = spnChild;
                                dicAccParent.Add(item.MENU_ID.ToString(), accParentMenu);
                                lstParentMenu.Add(accParentMenu);
                            }
                            i++;
                        }
                        else
                        {
                            hlkChild = GetHyperLinkButton(item.MENU_ID, menuNname, item.MENU_LINK, hyperlinkStyle);
                            //if ((dicAccParent[item.MENU_PARENT].Content as StackPanel).Children.Count > 0)
                            //    (dicAccParent[item.MENU_PARENT].Content as StackPanel).Children.Add(CreateBorder());
                            (dicAccParent[item.MENU_PARENT].Content as StackPanel).Children.Add(hlkChild);
                        }
                        if (!item.MENU_LINK.Equals("/") && !CurrentSystemInfor.AvailableLink.ContainsKey(item.MENU_LINK))
                            CurrentSystemInfor.AvailableLink.Add(item.MENU_LINK, item.MENU_ID.ToString());
                        //Chuc nang cho phep duyet tren tung menu hay khong
                        if (!CurrentSystemLogin.dicApproveFunction.ContainsKey(item.MENU_ID.ToString()))
                            CurrentSystemLogin.dicApproveFunction.Add(item.MENU_ID.ToString(), item.ISAPPROVE_FUNC.Equals("1") ? true : false);
                    }

                }
                if (CurrentSystemInfor.AvailableLink.Count == 0)
                    CurrentSystemInfor.AvailableLink.Add("/Home", "1");
                IsolatedStorageSettings.ApplicationSettings["MenuLink"] = CurrentSystemInfor.AvailableLink;
                this.MenuRoot = lstParentMenu;

               //Set lai IsApprove trong truong hop refresh
                if (CurrentSystemLogin.dicApproveFunction.ContainsKey(CurrentSystemInfor.CurrentMenuId))
                    CurrentSystemLogin.IsAppFunction = CurrentSystemLogin.dicApproveFunction[CurrentSystemInfor.CurrentMenuId];
            }
        }        

        public AccordionItem GetAccordionItemHyperlink(object header,int menuId, string uriLink, string uriIcon, Style style)
        {
            AccordionItem Items3 = new AccordionItem();
            Items3.Header = header;
            Items3.Content = uriLink;
            Items3.Selected += new RoutedEventHandler(accoclick);
            Items3.Style = style;
            Items3.TabIndex = menuId;
            return Items3;
        }        

        private AccordionItem GetAccordionItem(object header, Style style, string iconPath)
        {
            AccordionItem Items3 = new AccordionItem();
            Items3.Header = header;
            Items3.Style = style;            
            BitmapImage image = new BitmapImage();
            image.UriSource = new Uri(iconPath, UriKind.Relative);
            ImageSource ima = image;
            Items3.SetValue(EyeCandy.ImageProperty, ima);

            return Items3;
        }
        
        private HyperlinkButton GetHyperLinkButton(int menuId, string content, string uri, Style style)
        {
            HyperlinkButton hlkButton = new HyperlinkButton();
            hlkButton.Click += new RoutedEventHandler(hblkCliked);
            hlkButton.Content = content;
            hlkButton.TargetName = "ContentFrame";
            //hlkButton.Margin = new Thickness(15, 1, 0, 1);
            hlkButton.NavigateUri = new Uri(uri, UriKind.Relative);
            hlkButton.Style = style;
            hlkButton.TabIndex = menuId;
            //hlkButton.Width = 200;
            //hlkButton.Height = 32;
            //Luu vao list de quan ly state
            this.lstHyperlink.Add(hlkButton);

            return hlkButton;
        }

        
        private StackPanel CreateStackPanel()
        {
            StackPanel sp = new StackPanel();
            sp.Background = new SolidColorBrush(Color.FromArgb(255, 56, 155, 207));
            return sp;
        }

        //private AccordionItem GetAccordionItem(object header, Style style)
        //{
        //    AccordionItem Items3 = new AccordionItem();
        //    Items3.Header = header;
        //    //Items3.HorizontalAlignment = HorizontalAlignment.Left;
        //    Items3.AccordionButtonStyle = style;
        //    Items3.BorderBrush = new SolidColorBrush(Color.FromArgb(100, 204, 204, 204));
        //    Items3.Background = null;//new SolidColorBrush(Color.FromArgb(100, 110, 162, 207));
        //    Items3.Margin = new Thickness(0, 1, 0, 0);
        //    Items3.BorderThickness = new Thickness(0, 0, 0, 1);

        //    return Items3;
        //}

        //private AccordionItem GetAccordionItemParent(object header, Style style)
        //{
        //    AccordionItem Items3 = new AccordionItem();
        //    Items3.Header = header;
        //    //Items3.HorizontalAlignment = HorizontalAlignment.Left;
        //    Items3.AccordionButtonStyle = style;
        //    Items3.BorderBrush = new SolidColorBrush(Color.FromArgb(100, 204, 204, 204));
        //    Items3.Background = null;//new SolidColorBrush(Color.FromArgb(100, 110, 162, 207));
        //    Items3.Margin = new Thickness(0, 1, 0, 0);
        //    Items3.BorderThickness = new Thickness(0, 0, 0, 1);

        //    return Items3;
        //}

        //private HyperlinkButton GetHyperLinkButton(int menuId, string content, string uri, Style style)
        //{
        //    HyperlinkButton hlkButton = new HyperlinkButton();
        //    hlkButton.Click += new RoutedEventHandler(hblkCliked);
        //    hlkButton.Content = content;
        //    hlkButton.TargetName = "ContentFrame";
        //    hlkButton.Margin = new Thickness(15, 1, 0, 1);
        //    hlkButton.NavigateUri = new Uri(uri, UriKind.Relative);
        //    hlkButton.Style = style;
        //    hlkButton.TabIndex = menuId;            
        //    //Luu vao list de quan ly state
        //    this.lstHyperlink.Add(hlkButton);

        //    return hlkButton;
        //}

        //private HyperlinkButton GetHyperLinkParent(int menuId, string content, string uri, Style style)
        //{
        //    HyperlinkButton hlkButton = new HyperlinkButton();
        //    hlkButton.Click += new RoutedEventHandler(hblkCliked);
        //    hlkButton.Content = content;
        //    hlkButton.TargetName = "ContentFrame";
        //    hlkButton.Margin = new Thickness(20, 1, 0, 0);
        //    hlkButton.NavigateUri = new Uri(uri, UriKind.Relative);
        //    hlkButton.Style = style;
        //    hlkButton.TabIndex = menuId;
        //    //Luu vao list de quan ly state
        //    this.lstHyperlink.Add(hlkButton);

        //    return hlkButton;
        //}

        private Border CreateBorder()
        {
            Border boder = new Border();
            //boder.BorderBrush = new SolidColorBrush(Color.FromArgb(100, 204, 204, 204));
            //boder.BorderThickness = new Thickness(0, 0, 0, 1);
           // boder.Margin = new Thickness(15, 0, 0, 0);
            boder.MinWidth = 200;

            return boder;
        }

        private void accoclick(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings["MenuName"] = (sender as AccordionItem).Header.ToString();
            CurrentSystemInfor.CurrentMenuId = (sender as AccordionItem).TabIndex.ToString();
            IsolatedStorageSettings.ApplicationSettings["MenuId"] = CurrentSystemInfor.CurrentMenuId;
            foreach (var item in this.lstHyperlink)
            {
                VisualStateManager.GoToState(item, "InActive", false);
            }
        }

        private void hblkCliked(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings["MenuName"] = (sender as HyperlinkButton).Content.ToString();            
            CurrentSystemInfor.CurrentMenuId = (sender as HyperlinkButton).TabIndex.ToString();
            IsolatedStorageSettings.ApplicationSettings["MenuId"] = CurrentSystemInfor.CurrentMenuId;
            //foreach (var item in this.lstHyperlink)
            //    VisualStateManager.GoToState(item, "Unfocused", true);
            //VisualStateManager.GoToState((sender as HyperlinkButton), "Focused", true);

            foreach (var item in this.lstHyperlink)
                VisualStateManager.GoToState(item, "InActive", true);
            VisualStateManager.GoToState((sender as HyperlinkButton), "Active", true);  
        }

        private void setCurrentMenu()
        {
            foreach (var item in this.lstHyperlink)
            {
                string pathUri = System.Windows.Browser.HtmlPage.Document.DocumentUri.ToString();
                string currentUri = item.NavigateUri.ToString();
                if (pathUri.Substring(pathUri.Length - currentUri.Length).Equals(currentUri))
                {
                    VisualStateManager.GoToState(item, "Focused", true);
                    break;
                }
            }
        }

        private List<HyperlinkButton> lstHyperlink;       

        private List<AccordionItem> menuRoot;
        public List<AccordionItem> MenuRoot
        {
            get
            {
                return this.menuRoot;
            }

            set
            {
                this.menuRoot = value;
                this.OnPropertyChanged("MenuRoot");
            }
        }
    }
}
