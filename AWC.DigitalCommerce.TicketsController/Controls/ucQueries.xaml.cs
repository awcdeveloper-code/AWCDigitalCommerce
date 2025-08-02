using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AWC.DigitalCommerce.TicketsController.Properties;

namespace AWC.DigitalCommerce.TicketsController.Controls
{
    public partial class ucQueries : UserControl
    {
        private wpfMainWindow2 mw;
        
        private string lang = string.Empty;

        public ucQueries(wpfMainWindow _mw, string _lang)
        {
            lang = _lang;

            InitializeComponent();

            //Traductor.ApplyTranslation(this, lang);

            QryATV.IsEnabled = Settings.Default.ATVApplyFee;
        }

        public ucQueries(wpfMainWindow2 _mw, string _lang)
        {
            mw = _mw;
            lang = _lang;

            InitializeComponent();

            //Traductor.ApplyTranslation(this, lang);

            QryATV.IsEnabled = Settings.Default.ATVApplyFee;
        }

        #region TICKETS
        private void btn_TicketDetail(object sender, RoutedEventArgs e)
        {
            mw.Opacity = 0.5;
            wpfQryTicketByNumber wpfTicketbyNum = new wpfQryTicketByNumber(0);
            wpfTicketbyNum.ShowDialog();
            mw.Opacity = 1;
        }

        private void btn_DailyClose(object sender, RoutedEventArgs e)
        {
            mw.Opacity = 0.5;
            wpfQryDailyClose wpfDayClose = new wpfQryDailyClose();
            wpfDayClose.ShowDialog();
            mw.Opacity = 1;
        }

        private void btn_KitchenClose(object sender, RoutedEventArgs e)
        {
            mw.Opacity = 0.5;
            wpfQryKitchenClose wpfKitClose = new wpfQryKitchenClose();
            wpfKitClose.ShowDialog();
            mw.Opacity = 1;
        }

        private void btn_SalesHistory(object sender, RoutedEventArgs e)
        {
            mw.Opacity = 0.5;
            wpfSalesHistory wpfSalesHist = new wpfSalesHistory();
            wpfSalesHist.ShowDialog();
            mw.Opacity = 1;
        }

        private void btn_TicketsSummary(object sender, RoutedEventArgs e)
        {
            mw.Opacity = 0.5;
            wpfQryTicketsSummary tckSumm = new wpfQryTicketsSummary();
            tckSumm.ShowDialog();
            mw.Opacity = 1;
        }
        #endregion

        #region CUSTOMERS
        private void btn_CustomersVIP(object sender, RoutedEventArgs e)
        {
            mw.Opacity = 0.5;
            wpfQryCustomersID wpfCustID = new wpfQryCustomersID();
            wpfCustID.ShowDialog();
            mw.Opacity = 1;
        }

        private void btn_TicketsByVIP(object sender, RoutedEventArgs e)
        {
            mw.Opacity = 0.5;
            wpfQryTicketsByCustomerID wpfTcksByCustID = new wpfQryTicketsByCustomerID();
            wpfTcksByCustID.ShowDialog();
            mw.Opacity = 1;
        }

        private void btn_Delinquency(object sender, RoutedEventArgs e)
        {
            mw.Opacity = 0.5;
            wpfDelinquency delinquency = new wpfDelinquency();
            delinquency.ShowDialog();
            mw.Opacity = 1;
        }

        private void btn_Payments(object sender, RoutedEventArgs e)
        {
            mw.Opacity = 0.5;
            wpfSmallPayments smallPayments = new wpfSmallPayments();
            smallPayments.ShowDialog();
            mw.Opacity = 1;
        }
        #endregion

        #region MISCELLANEUS
        private void btn_ItemsPriceList(object sender, RoutedEventArgs e)
        {
            mw.Opacity = 0.5;
            wpfItemsPriceList wpfIPL = new wpfItemsPriceList();
            wpfIPL.ShowDialog();
            mw.Opacity = 1;
        }

        private void btn_ItemTypeSummary(object sender, RoutedEventArgs e)
        {
            mw.Opacity = 0.5;
            wpfQryConsumption wpfConsum = new wpfQryConsumption();
            wpfConsum.ShowDialog();
            mw.Opacity = 1;
        }

        private void btn_Graphics(object sender, RoutedEventArgs e)
        {
            mw.Opacity = 0.5;
            wpfQryGraphics wpfGraph = new wpfQryGraphics();
            wpfGraph.ShowDialog();
            mw.Opacity = 1;
        }

        private void btn_Inventory(object sender, RoutedEventArgs e)
        {
            mw.Opacity = 0.5;
            wpfSQLQuery sqlQry = new wpfSQLQuery();
            sqlQry.ShowDialog();
            mw.Opacity = 1;
        }

        private void btn_ActivityLog(object sender, RoutedEventArgs e)
        {
            mw.Opacity = 0.5;
            wpfWorkLog wpfLog = new wpfWorkLog();
            wpfLog.ShowDialog();
            mw.Opacity = 1;
        }

        private void btn_Providers(object sender, RoutedEventArgs e)
        {
            mw.Opacity = 0.5;
            wpfProviders wpfProv = new wpfProviders();
            wpfProv.ShowDialog();
            mw.Opacity = 1;
        }
        #endregion

        private void btn_ApplyServiceFee(object sender, RoutedEventArgs e)
        {
            mw.Opacity = 0.5;
            wpfServiceFeeSummary sfSumm = new wpfServiceFeeSummary();
            sfSumm.ShowDialog();
            mw.Opacity = 1;
        }

        private void btn_TicketsAborted(object sender, RoutedEventArgs e)
        {
            mw.Opacity = 0.5;
            wpfTicketsAborted tckAborted = new wpfTicketsAborted();
            tckAborted.ShowDialog();
            mw.Opacity = 1;
        }

        private void btn_TicketsModified(object sender, RoutedEventArgs e)
        {
            mw.Opacity = 0.5;
            wpfTicketsModified tckModified = new wpfTicketsModified();
            tckModified.ShowDialog();
            mw.Opacity = 1;
        }

        private void btn_TicketsLost(object sender, RoutedEventArgs e)
        {
            mw.Opacity = 0.5;
            wpfTicketsLost tckLost = new wpfTicketsLost();
            tckLost.ShowDialog();
            mw.Opacity = 1;
        }

        private void btn_ReportsByMail(object sender, RoutedEventArgs e)
        {
            mw.Opacity = 0.5;
            wpfSendReportByEMail sndRep = new wpfSendReportByEMail();
            sndRep.ShowDialog();
            mw.Opacity = 1;
        }

        private void btn_TicketsInherited(object sender, RoutedEventArgs e)
        {
            mw.Opacity = 0.5;
            wpfTicketsInherited tckInherited = new wpfTicketsInherited();
            tckInherited.ShowDialog();
            mw.Opacity = 1;
        }

        private void btn_TicketsReassigned(object sender, RoutedEventArgs e)
        {
            mw.Opacity = 0.5;
            wpfTicketsReassigned tckReassigned = new wpfTicketsReassigned();
            tckReassigned.ShowDialog();
            mw.Opacity = 1;
        }

        private void btn_ItemsDeleted(object sender, RoutedEventArgs e)
        {
            mw.Opacity = 0.5;
            wpfItemesDeleted itemdel = new wpfItemesDeleted();
            itemdel.ShowDialog();
            mw.Opacity = 1;
        }

        private void btn_QryATV(object sender, RoutedEventArgs e)
        {
            mw.Opacity = 0.5;
            wpfIVAQuery ivaQry = new wpfIVAQuery();
            ivaQry.ShowDialog();
            mw.Opacity = 1;
        }

        private void btn_IncomeCash(object sender, RoutedEventArgs e)
        {
            mw.Opacity = 0.5;
            wpfIncomeCash iCash = new wpfIncomeCash();
            iCash.ShowDialog();
            mw.Opacity = 1;
        }

        private void btn_CocktailRecipes(object sender, RoutedEventArgs e)
        {
            mw.Opacity = 0.5;
            wpfCocktailRecipes cocktailRecipes = new wpfCocktailRecipes();
            cocktailRecipes.ShowDialog();
            mw.Opacity = 1;
        }

        private void btn_Analitics(object sender, RoutedEventArgs e)
        {
            mw.Opacity = 0.5;
            wpfTheFiveMostRequested theMostRequested = new wpfTheFiveMostRequested();
            theMostRequested.ShowDialog();
            mw.Opacity = 1;
        }

        private void btn_Explorer(object sender, RoutedEventArgs e)
        {
            Helper.InDevelopment();
        }

        private void btn_DeletedFromSystem(object sender, RoutedEventArgs e)
        {
            mw.Opacity = 0.5;
            wpfItemesDeletedFromSystem itemdel = new wpfItemesDeletedFromSystem();
            itemdel.ShowDialog();
            mw.Opacity = 1;
        }
    }
}
