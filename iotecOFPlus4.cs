#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.DrawingTools;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Threading;
#endregion

namespace NinjaTrader.NinjaScript.Indicators.lab
{
	public class iotecOFPlus4 : Indicator
	{
		#region Vars
		private Gui.Tools.SimpleFont font;
        private bool needsLayoutUpdate;
		private int aggregationInterval;
		private NinjaTrader.NinjaScript.BarsTypes.VolumetricBarsType barsType;
		private SessionIterator sessionIterator;
		
		private DateTime sessionEnd;
		
		private int startSessBarIndex = 0;
		private int endSessBarIndex = 0;
		
		private int		sessionCounter;
		private double 	highOD;
		private double 	lowOD;
		
		private float 	deltaAv;
		private float	sumDeltas;
		private int		barsInSess;
		
		//Dictionaries
		private Dictionary<int, Dictionary<double, double>> GetAskVolumeForPrice = new Dictionary<int, Dictionary<double, double>>();
        private Dictionary<int, Dictionary<double, double>> GetBidVolumeForPrice = new Dictionary<int, Dictionary<double, double>>();
		
		private Dictionary<int, Dictionary<double, double>> GetDeltaForPrice = new Dictionary<int, Dictionary<double, double>>();
		private Dictionary<int, Dictionary<double, double>> GetTotalVolumeForPrice = new Dictionary<int, Dictionary<double, double>>();
		private Dictionary<int, double> GetMaximumVolume = new Dictionary<int, double>();
		private Dictionary<int, double> GetMaximumVolumeVP = new Dictionary<int, double>();
		
		private Dictionary<int, double> GetMaximumAskVolume = new Dictionary<int, double>();
        private Dictionary<int, double> GetMaximumBidVolume = new Dictionary<int, double>();
		
		private Dictionary<int, double> GetMaximumPositiveDelta = new Dictionary<int, double>();
        private Dictionary<int, double> GetMaximumNegativeDelta = new Dictionary<int, double>();
		
		private Dictionary<int, Dictionary<double, bool>> bidVolumeImbDictD = new Dictionary<int, Dictionary<double, bool>>();
        private Dictionary<int, Dictionary<double, bool>> askVolumeImbDictD = new Dictionary<int, Dictionary<double, bool>>();
		private Dictionary<int, Dictionary<double, bool>> bidVolumeImbDictH = new Dictionary<int, Dictionary<double, bool>>();
        private Dictionary<int, Dictionary<double, bool>> askVolumeImbDictH = new Dictionary<int, Dictionary<double, bool>>();
		
        private Dictionary<int, Dictionary<double, bool>> bidStckImbDictD = new Dictionary<int, Dictionary<double, bool>>();
        private Dictionary<int, Dictionary<double, bool>> askStckImbDictD = new Dictionary<int, Dictionary<double, bool>>();
        private Dictionary<int, Dictionary<double, bool>> bidStckImbDictH = new Dictionary<int, Dictionary<double, bool>>();
        private Dictionary<int, Dictionary<double, bool>> askStckImbDictH = new Dictionary<int, Dictionary<double, bool>>();
		
		private Dictionary<int, Dictionary<double, double>> VP = new Dictionary<int, Dictionary<double, double>>();
				
		private Dictionary<int, double> VAH = new Dictionary<int, double>();
        private Dictionary<int, double> VAL = new Dictionary<int, double>();
		
		private Dictionary<int, double> BarRatio = new Dictionary<int, double>(); 
		
		private Dictionary<int, int> HOD = new Dictionary<int, int>();
		private Dictionary<int, int> LOD = new Dictionary<int, int>();
		
		private Dictionary<int, Dictionary<int, double>> BigTrades = new Dictionary<int, Dictionary<int, double>>();
		private Dictionary<DateTime, Dictionary<int, double>> BigTradesT = new Dictionary<DateTime, Dictionary<int, double>>();
		
		private Series<float> deltaAverage;
						
		//Render
		private SharpDX.Vector2							reuseVector, reuseVector2;
		private SharpDX.DirectWrite.TextFormat			reuseTextFormat;
		private SharpDX.DirectWrite.TextLayout			reuseTextLayout;
		private SharpDX.RectangleF						reuseRect;
		
		//Visuals
		
		private System.Windows.Controls.Image iconImage;
//		private System.Windows.Controls.Image iconMenuTypeFPHist;
//		private System.Windows.Controls.Image iconMenuTypeFPBA;
		
		private NinjaTrader.Gui.Chart.ChartTab		chartTab;
		private NinjaTrader.Gui.Chart.Chart			chartWindow;
		private bool								isToolBarButtonAdded;
		private System.Windows.DependencyObject		searchObject;
		private System.Windows.Controls.TabItem		tabItem;
		private System.Windows.Controls.Menu		theMenu;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItem;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuTYpeOF;
		
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem1;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem11;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem12;
		
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem2;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem21;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem22;
		
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem3;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem31;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem32;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem33;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem34;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem35;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem36;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem37;
		
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem4;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem41;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem42;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem43;
		
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem44;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem441;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem442;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem443;
		
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem45;
		
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem5;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem6;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem7;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem8;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem9;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem10;

		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem13;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem14;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem15;
		
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem16;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem17;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem18;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem19;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem20;

		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem23;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem24;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem25;
		
		#endregion
		
		#region Props
		[Display(Name = "Type of footprint (checked BidAsk)", GroupName = "0.Settings", Order = 0)]
		public bool IsBidAsk
		{ get; set; }
		
		[Display(Name = "Ticks per level", GroupName = "0.Settings", Order = 1)]
        public int AggregationInterval
        {
            get { return aggregationInterval; }
            set { aggregationInterval = Math.Max(1, value); }
        }
		
        [Range(1, 100)]
        [Display(Name = "Bar width", GroupName = "0.Settings", Order = 2)]
        public int BarWidth { get; set; }


        [Display(ResourceType = typeof(Custom.Resource), Name = "Font", GroupName = "0.Settings", Order = 4)]
        public Gui.Tools.SimpleFont Font
        {
            get { return font; }
            set
            {
                font = value;
                needsLayoutUpdate = true;
            }
        }
		
		[Range(1, 10000)]
        [Display(Name = "VP width", GroupName = "0.Settings", Order = 5)]
        public int VPWidth { get; set; }
		//totalBarsSession
		
		[Range(1, 10)]
        [Display(Name = "Factor of imbalances", GroupName = "1.Internals", Order = 1)]
        public double ImbFact { get; set; }

        [Range(1, 10000)]
        [Display(Name = "Vol min for mayor value", GroupName = "1.Internals", Order = 2)]
        public double MinVolH { get; set; }

        [Range(0, 10000)]
        [Display(Name = "Vol min for minor value", GroupName = "1.Internals", Order = 3)]
        public double MinVolL { get; set; }

        [Range(1, 10000)]
        [Display(Name = "Vol max for minor value", GroupName = "1.Internals", Order = 4)]
        public double MaxVolL { get; set; }

		[Display(Name = "Type of Imbalances (checked Diagonal)", GroupName = "0.Settings", Order = 5)]
		public bool ImbType
		{ get; set; }
		
//        [Display(Name = "Type of imbalances", GroupName = "1.Internals", Order = 5)]
//        public ImbType MyImbType { get; set; }

//        public enum ImbType
//        {
//            Diagonal,
//            Horizontal
//        }

        [Range(2, 10)]
        [Display(Name = "Number of stacked imbalances", GroupName = "1.Internals", Order = 6)]
        public int StckImbNum { get; set; }
		
		[Range(1, 50)]
		[Display(Name = "Ratio high", GroupName = "1.Internals", Order = 7)]
		public double RatioH
		{ get; set; }
		
		[Range(0, 1)]
		[Display(Name = "Ratio low", GroupName = "1.Internals", Order = 8)]
		public double RatioL
		{ get; set; }
		
	    [Range(1, 100)]
        [Display(Name = "Value area %", GroupName = "1.Internals", Order = 9)]
        public double ValueAreaPer { get; set; }
		
		[Range(1, 20000)]
		[Display(Name = "BigTrades volume", GroupName = "1.Internals", Order = 10)]
		public int BTVol
		{ get; set; }
		
		[Range(1, 10)]
		[Display(Name = "BigTrades max factor volume for SR lines", GroupName = "1.Internals", Order = 11)]
		public double BTVolHVLSR
		{ get; set; }
		
		[Range(1, 10)]
		[Display(Name = "BigTrades min factor volume for SR lines", GroupName = "1.Internals", Order = 12)]
		public double BTVolLVLSR
		{ get; set; }
		
		[Display(Name = "BigTrades text for SR lines", GroupName = "1.Internals", Order = 12)]
		public bool BTLSRText
		{ get; set; }
		
		[Range(1, 100000)]
		[Display(Name = "Big volume by bar", GroupName = "1.Internals", Order = 13)]
		public int BigBarVol
		{ get; set; }
		
        // Colors
        [XmlIgnore]
        [NinjaScriptProperty]
        [Display(Name = "Bullish Bar", GroupName = "2.Colors", Order = 0)]
        public System.Windows.Media.Brush BarUpColor { get; set; }

        [Browsable(false)]
        public string BarUpColorSerialize
        {
            get { return Serialize.BrushToString(BarUpColor); }
            set { BarUpColor = Serialize.StringToBrush(value); }
        }

        [XmlIgnore]
        [NinjaScriptProperty]
        [Display(Name = "Bearish Bar", GroupName = "2.Colors", Order = 1)]
        public System.Windows.Media.Brush BarDownColor { get; set; }

        [Browsable(false)]
        public string BarDownColorSerialize
        {
            get { return Serialize.BrushToString(BarDownColor); }
            set { BarDownColor = Serialize.StringToBrush(value); }
        }

        [XmlIgnore]
        [NinjaScriptProperty]
        [Display(Name = "Doji Bar", GroupName = "2.Colors", Order = 2)]
        public System.Windows.Media.Brush DojiColor { get; set; }

        [Browsable(false)]
        public string DojiColorSerialize
        {
            get { return Serialize.BrushToString(DojiColor); }
            set { DojiColor = Serialize.StringToBrush(value); }
        }

        [XmlIgnore]
        [NinjaScriptProperty]
        [Display(Name = "Wick", GroupName = "2.Colors", Order = 3)]
        public System.Windows.Media.Brush WickColor { get; set; }

        [Browsable(false)]
        public string WickColorSerialize
        {
            get { return Serialize.BrushToString(WickColor); }
            set { WickColor = Serialize.StringToBrush(value); }
        }

        [XmlIgnore]
        [NinjaScriptProperty]
        [Display(Name = "PoC", GroupName = "2.Colors", Order = 4)]
        public System.Windows.Media.Brush PocColor { get; set; }

        [Browsable(false)]
        public string PocColorSerialize
        {
            get { return Serialize.BrushToString(PocColor); }
            set { PocColor = Serialize.StringToBrush(value); }
        }

        [XmlIgnore]
        [NinjaScriptProperty]
        [Display(Name = "Delta Positive", GroupName = "2.Colors", Order = 5)]
        public System.Windows.Media.Brush DeltaPosColor { get; set; }

        [Browsable(false)]
        public string DeltaPosColorSerialize
        {
            get { return Serialize.BrushToString(DeltaPosColor); }
            set { DeltaPosColor = Serialize.StringToBrush(value); }
        }

        [XmlIgnore]
        [NinjaScriptProperty]
        [Display(Name = "Delta Negative", GroupName = "2.Colors", Order = 6)]
        public System.Windows.Media.Brush DeltaNegColor { get; set; }

        [Browsable(false)]
        public string DeltaNegColorSerialize
        {
            get { return Serialize.BrushToString(DeltaNegColor); }
            set { DeltaNegColor = Serialize.StringToBrush(value); }
        }

        [XmlIgnore]
        [NinjaScriptProperty]
        [Display(Name = "Text", GroupName = "2.Colors", Order = 7)]
        public System.Windows.Media.Brush TextColor { get; set; }

        [Browsable(false)]
        public string TextColorSerialize
        {
            get { return Serialize.BrushToString(TextColor); }
            set { TextColor = Serialize.StringToBrush(value); }
        }

        [XmlIgnore]
        [NinjaScriptProperty]
        [Display(Name = "Ask Imbalance", GroupName = "2.Colors", Order = 8)]
        public System.Windows.Media.Brush AskImbColor { get; set; }

        [Browsable(false)]
        public string AskImbColorSerialize
        {
            get { return Serialize.BrushToString(AskImbColor); }
            set { AskImbColor = Serialize.StringToBrush(value); }
        }

        [XmlIgnore]
        [NinjaScriptProperty]
        [Display(Name = "Bid Imbalance", GroupName = "2.Colors", Order = 9)]
        public System.Windows.Media.Brush BidImbColor { get; set; }

        [Browsable(false)]
        public string BidImbColorSerialize
        {
            get { return Serialize.BrushToString(BidImbColor); }
            set { BidImbColor = Serialize.StringToBrush(value); }
        }

        [XmlIgnore]
        [NinjaScriptProperty]
        [Display(Name = "Stcked Ask Imbalance", GroupName = "2.Colors", Order = 10)]
        public System.Windows.Media.Brush StckAskImbColor { get; set; }

        [Browsable(false)]
        public string StckAskImbColorSerialize
        {
            get { return Serialize.BrushToString(StckAskImbColor); }
            set { StckAskImbColor = Serialize.StringToBrush(value); }
        }

        [XmlIgnore]
        [NinjaScriptProperty]
        [Display(Name = "Stcked Bid Imbalance", GroupName = "2.Colors", Order = 11)]
        public System.Windows.Media.Brush StckBidImbColor { get; set; }

        [Browsable(false)]
        public string StckBidImbColorSerialize
        {
            get { return Serialize.BrushToString(StckBidImbColor); }
            set { StckBidImbColor = Serialize.StringToBrush(value); }
        }

        [XmlIgnore]
        [NinjaScriptProperty]
        [Display(Name = "UnFinish Auction", GroupName = "2.Colors", Order = 12)]
        public System.Windows.Media.Brush UnFinishAuctColor { get; set; }

        [Browsable(false)]
        public string UnFinishAuctColorSerialize
        {
            get { return Serialize.BrushToString(UnFinishAuctColor); }
            set { UnFinishAuctColor = Serialize.StringToBrush(value); }
        }

        [XmlIgnore]
        [NinjaScriptProperty]
        [Display(Name = "Volume", GroupName = "2.Colors", Order = 13)]
        public System.Windows.Media.Brush VolColor { get; set; }

        [Browsable(false)]
        public string VolColorSerialize
        {
            get { return Serialize.BrushToString(VolColor); }
            set { VolColor = Serialize.StringToBrush(value); }
        }

        [XmlIgnore]
        [NinjaScriptProperty]
        [Display(Name = "Bid Volume", GroupName = "2.Colors", Order = 14)]
        public System.Windows.Media.Brush VolBidColor { get; set; }

        [Browsable(false)]
        public string VolBidColorSerialize
        {
            get { return Serialize.BrushToString(VolBidColor); }
            set { VolBidColor = Serialize.StringToBrush(value); }
        }

        [XmlIgnore]
        [NinjaScriptProperty]
        [Display(Name = "Ask Volume", GroupName = "2.Colors", Order = 15)]
        public System.Windows.Media.Brush VolAskColor { get; set; }

        [Browsable(false)]
        public string VolAskColorSerialize
        {
            get { return Serialize.BrushToString(VolAskColor); }
            set { VolAskColor = Serialize.StringToBrush(value); }
        }
		
        [XmlIgnore]
        [NinjaScriptProperty]
        [Display(Name = "Value Area (VA)", GroupName = "2.Colors", Order = 16)]
        public System.Windows.Media.Brush VAColor { get; set; }

        [Browsable(false)]
        public string VAColorSerialize
        {
            get { return Serialize.BrushToString(VAColor); }
            set { VAColor = Serialize.StringToBrush(value); }
        }
		
        [XmlIgnore]
        [NinjaScriptProperty]
        [Display(Name = "Session Volume Profile", GroupName = "2.Colors", Order = 17)]
        public System.Windows.Media.Brush VPColor { get; set; }

        [Browsable(false)]
        public string VPColorSerialize
        {
            get { return Serialize.BrushToString(VPColor); }
            set { VPColor = Serialize.StringToBrush(value); }
        }
		
		[XmlIgnore]
        [NinjaScriptProperty]
        [Display(Name = "High Absorption BID", GroupName = "2.Colors", Order = 18)]
        public System.Windows.Media.Brush AbsorptionBidColor { get; set; }

        [Browsable(false)]
        public string AbsorptionBidColorSerialize
        {
            get { return Serialize.BrushToString(AbsorptionBidColor); }
            set { AbsorptionBidColor = Serialize.StringToBrush(value); }
        }
		
		[XmlIgnore]
        [NinjaScriptProperty]
        [Display(Name = "High Absorption ASK", GroupName = "2.Colors", Order = 19)]
        public System.Windows.Media.Brush AbsorptionAskColor { get; set; }

        [Browsable(false)]
        public string AbsorptionAskColorSerialize
        {
            get { return Serialize.BrushToString(AbsorptionAskColor); }
            set { AbsorptionAskColor = Serialize.StringToBrush(value); }
        }
		
		///Visuals
		[Display(Name = "Show Text BID Side", GroupName = "3.Visuals", Order = 0)]
		public bool ShowTextB

		{ get; set; }
		
		[Display(Name = "Show Text ASK Side", GroupName = "3.Visuals", Order = 0)]
		public bool ShowTextA

		{ get; set; }
		
		[Display(Name = "Show Text Delta Side", GroupName = "3.Visuals", Order = 0)]
		public bool ShowTextD

		{ get; set; }
		
		[Display(Name = "Show Text Volume Side", GroupName = "3.Visuals", Order = 0)]
		public bool ShowTextV

		{ get; set; }
		
		[Display(Name = "Show Imbalances", GroupName = "3.Visuals", Order = 1)]
		public bool ShowImb
		{ get; set; }
		
		[Display(Name = "Show Stacked Imbalances", GroupName = "3.Visuals", Order = 2)]
		public bool ShowStckImb
		{ get; set; }
		
		[Display(Name = "Show Ratios", GroupName = "3.Visuals", Order = 3)]
		public bool ShowRatios
		{ get; set; }
		
		[Display(Name = "Show Volume Histogram", GroupName = "3.Visuals", Order = 4)]
		public bool ShowHistVol
		{ get; set; }
		
		[Display(Name = "Show Delta Histogram", GroupName = "3.Visuals", Order = 5)]
		public bool ShowHistDelta
		{ get; set; }
		
		[Display(Name = "Show Parameters info", GroupName = "3.Visuals", Order = 6)]
		public bool ShowParInfo
		{ get; set; }
		
		[Display(Name = "Show Summary table", GroupName = "3.Visuals", Order = 7)]
		public bool ShowSummTab
		{ get; set; }
		
//		[Display(Name = "Show Big Trades", GroupName = "3.Visuals", Order = 8)]
//		public bool ShowBT
//		{ get; set; }	
		
		[Display(Name = "Show Big Trades Circles", GroupName = "3.Visuals", Order = 8)]
		public bool ShowBTCircles
		{ get; set; }
		
		[Display(Name = "Show Big Trades Lines S/R", GroupName = "3.Visuals", Order = 8)]
		public bool ShowBTLSR
		{ get; set; }
		
		[Display(Name = "Show HOD/LOD", GroupName = "3.Visuals", Order = 9)]
		public bool ShowHLOD
		{ get; set; }
		
		[Display(Name = "Bar separation inactive", GroupName = "3.Visuals", Order = 10)]
		public bool MinWidth
		{ get; set; }
		
		[Display(Name = "Show Session VP", GroupName = "3.Visuals", Order = 11)]
		public bool ShowVP
		{ get; set; }
		
		[Display(Name = "Show Absorptions", GroupName = "3.Visuals", Order = 12)]
		public bool ShowABS
		{ get; set; }
		
		[Display(Name = "Show Value Area", GroupName = "3.Visuals", Order = 13)]
		public bool ShowVA
		{ get; set; }
		
		[Display(Name = "Show Delta Bars Histogram", GroupName = "3.Visuals", Order = 14)]
		public bool ShowDBH
		{ get; set; }
		#endregion
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Indicator here.";
				Name										= "iotecOFPlus4";
				Calculate									= Calculate.OnEachTick;
				IsOverlay									= true;
				DisplayInDataBox							= true;
				DrawOnPricePanel							= true;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= true;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive					= false;
				
				Font 										= new Gui.Tools.SimpleFont() { Size = 12 };
				AggregationInterval 						= 1;
 				BarWidth 									= 5;
				ImbFact 									= 4;
                MinVolH 									= 1;
                MinVolL										= 0;
                MaxVolL										= 2000;
                StckImbNum 									= 3;
				ImbType										= true;
//                MyImbType 									= ImbType.Diagonal;
				RatioH										= 30;
				RatioL										= 0.69;
                ValueAreaPer 								= 70;
				BTVol										= 2000;
				BTVolHVLSR 									= 2.5;
				BTVolLVLSR 									= 1.5;
				BTLSRText									= true;
				BigBarVol									= 10000;
				VPWidth										= 20;
				
				IsBidAsk 									= true;
				ShowTextB									= true;
				ShowTextA									= true;
				ShowTextD									= true;
				ShowTextV									= true;									
				ShowImb										= true;
				ShowStckImb 								= true;
				ShowRatios 									= true;
//				ShowSummTab 								= true;
				ShowHistVol 								= true;
				ShowHistDelta 								= true;
//				ShowParInfo									= false;
//				ShowBT 										= true;
				ShowBTCircles 								= true;
				ShowBTLSR 									= true;
				ShowHLOD 									= true;
				MinWidth 									= true;
				ShowVP 										= true;
				ShowABS 									= true;
				ShowVA										= true;
				ShowDBH										= true;
				
                BarUpColor = Brushes.DeepSkyBlue;
                BarDownColor = Brushes.DeepPink;
                DojiColor = Brushes.DimGray;
                WickColor = Brushes.DimGray;
                PocColor = Brushes.Orange;
                DeltaPosColor = Brushes.DeepSkyBlue;
                DeltaNegColor = Brushes.DeepPink;
                TextColor = Brushes.White;
                AskImbColor = Brushes.Green;
                BidImbColor = Brushes.Red;
                StckAskImbColor = Brushes.Gold;
                StckBidImbColor = Brushes.HotPink;
                UnFinishAuctColor = Brushes.Yellow;
                VolColor = Brushes.DimGray;
                VolBidColor = Brushes.DodgerBlue;
                VolAskColor = Brushes.Crimson;
				VAColor = Brushes.Silver;
				VPColor = Brushes.White;
				AbsorptionBidColor = Brushes.Orange;
				AbsorptionAskColor = Brushes.Lime;
			}
			else if (State == State.Configure)
			{
				ClearOutputWindow();
				AddVolumetric(Instrument.FullName, BarsPeriod.BarsPeriodType, BarsPeriod.Value, VolumetricDeltaType.BidAsk, 1);
			}
			else if (State == State.DataLoaded)
			{
				barsType = BarsArray[1].BarsType as	NinjaTrader.NinjaScript.BarsTypes.VolumetricBarsType;	
				
				deltaAverage = new Series<float>(this);
				
				sessionIterator = new SessionIterator(Bars);
			}
            else if (State == State.Historical)
            {
				if (ChartControl != null && !isToolBarButtonAdded)
				{
					ChartControl.Dispatcher.InvokeAsync((Action)(() =>
					{
						InsertWPFControls();
					}));
				}
            }
			else if (State == State.Terminated)
			{
				if (ChartControl != null)
				{
					ChartControl.Dispatcher.InvokeAsync((Action)(() =>
					{
						RemoveWPFControls();
					}));
				}
			}
		}
		
		protected override void OnBarUpdate()
		{
			try
			{
				if (Bars == null || barsType == null)
					return;

				if (Bars.IsFirstBarOfSession)
				{		
					if(IsFirstTickOfBar)
						sessionEnd = sessionIterator.ActualSessionEnd;
					
					barsInSess = 0;
					sumDeltas = 0;
					deltaAv = 0;
//					deltaAverage[0] = Math.Abs(barsType.Volumes[CurrentBar].BarDelta);
					
					startSessBarIndex = CurrentBar;
					
					sessionCounter++;
					highOD = High[0];
					lowOD = Low[0];
					
					// Make sure the first bar is stored in HOD and LOD at the start of the session
				    HOD[sessionCounter] = -1;
				    LOD[sessionCounter] = -1;
					
				}
				else
				{							
					if(High[0] > highOD)
					{
						highOD = High[0];
						HOD[sessionCounter] = CurrentBar;
					}
					
					if(Low[0] < lowOD)
					{
						lowOD = Low[0];
						LOD[sessionCounter] = CurrentBar;
						LOD[sessionCounter] = LOD[sessionCounter] == -1 ? startSessBarIndex : CurrentBar;
					}
					
					if(HOD[sessionCounter] == -1)
						HOD[sessionCounter] = startSessBarIndex;
					if(LOD[sessionCounter] == -1)
						LOD[sessionCounter] = startSessBarIndex;
				}
				
				if(BarsInProgress == 1 && IsFirstTickOfBar)
				{
					barsInSess++;	
					sumDeltas += Math.Abs(barsType.Volumes[CurrentBar].BarDelta);
					deltaAv = sumDeltas/barsInSess;
					deltaAverage[0] = deltaAv;
//					Print(sumDeltas + " " + barsInSess + " " + (sumDeltas/barsInSess));
				}
				
				if (Bars.IsLastBarOfSession)
					endSessBarIndex = CurrentBar;
				else
					endSessBarIndex = CurrentBar;
									
				//Hide candles
				BarBrush = Brushes.Transparent;
				CandleOutlineBrush =  Brushes.Transparent;	
				
				BarBrushes[-1] = Brushes.Transparent;
				CandleOutlineBrushes[-1] =  Brushes.Transparent;
				
				//Initialize all the dictionaries			
			    if (!GetAskVolumeForPrice.ContainsKey(CurrentBar))
	                GetAskVolumeForPrice[CurrentBar] = new Dictionary<double, double>();
	
	            if (!GetBidVolumeForPrice.ContainsKey(CurrentBar))
	                GetBidVolumeForPrice[CurrentBar] = new Dictionary<double, double>();
				
	            if (!GetDeltaForPrice.ContainsKey(CurrentBar))
	                GetDeltaForPrice[CurrentBar] = new Dictionary<double, double>();
	
	            if (!GetTotalVolumeForPrice.ContainsKey(CurrentBar))
	                GetTotalVolumeForPrice[CurrentBar] = new Dictionary<double, double>();
				
		        if (!bidVolumeImbDictD.ContainsKey(CurrentBar))
		            bidVolumeImbDictD[CurrentBar] = new Dictionary<double, bool>();
		
		        if (!askVolumeImbDictD.ContainsKey(CurrentBar))
		            askVolumeImbDictD[CurrentBar] = new Dictionary<double, bool>();
				
		        if (!bidVolumeImbDictH.ContainsKey(CurrentBar))
		            bidVolumeImbDictH[CurrentBar] = new Dictionary<double, bool>();
		
		        if (!askVolumeImbDictH.ContainsKey(CurrentBar))
		            askVolumeImbDictH[CurrentBar] = new Dictionary<double, bool>();
		
		        if (!bidStckImbDictD.ContainsKey(CurrentBar))
		            bidStckImbDictD[CurrentBar] = new Dictionary<double, bool>();
		
		        if (!askStckImbDictD.ContainsKey(CurrentBar))
		            askStckImbDictD[CurrentBar] = new Dictionary<double, bool>();
				
		        if (!bidStckImbDictH.ContainsKey(CurrentBar))
		            bidStckImbDictH[CurrentBar] = new Dictionary<double, bool>();
		
		        if (!askStckImbDictH.ContainsKey(CurrentBar))
		            askStckImbDictH[CurrentBar] = new Dictionary<double, bool>();
				
				if (!GetMaximumPositiveDelta.ContainsKey(CurrentBar))
	                GetMaximumPositiveDelta[CurrentBar] = double.MinValue;
	
	            if (!GetMaximumNegativeDelta.ContainsKey(CurrentBar))
	                GetMaximumNegativeDelta[CurrentBar] = double.MaxValue;
				
		        if (!VP.ContainsKey(startSessBarIndex))
		            VP[startSessBarIndex] = new Dictionary<double, double>();
				
		        if (!BigTrades.ContainsKey(endSessBarIndex))
		            BigTrades[endSessBarIndex] = new Dictionary<int, double>();
				
		        if (!BigTradesT.ContainsKey(sessionEnd))
		            BigTradesT[sessionEnd] = new Dictionary<int, double>();
				
				// Aggregation and volume update logic here
				double tickSizeInterval = AggregationInterval * TickSize;
				double highPrice 	= High[0];
				double lowPrice 	= Low[0];
		
				for (double p = lowPrice; p <= highPrice; p += tickSizeInterval)
	            {
	                double aggregatedAskVolume = 0;
	                double aggregatedBidVolume = 0;
	
	                for (double currentPrice = p; currentPrice < p + tickSizeInterval && currentPrice <= highPrice; currentPrice += TickSize)
	                {
						aggregatedAskVolume += barsType.Volumes[CurrentBar].GetAskVolumeForPrice(currentPrice);
						aggregatedBidVolume += barsType.Volumes[CurrentBar].GetBidVolumeForPrice(currentPrice);
	                }
	
	                GetAskVolumeForPrice[CurrentBar][p] 	= aggregatedAskVolume;
	                GetBidVolumeForPrice[CurrentBar][p] 	= aggregatedBidVolume;
					GetDeltaForPrice[CurrentBar][p] 		= aggregatedAskVolume - aggregatedBidVolume;
					GetTotalVolumeForPrice[CurrentBar][p] 	= aggregatedAskVolume + aggregatedBidVolume;
					
//					VP[startSessBarIndex][p]				= aggregatedAskVolume + aggregatedBidVolume;
	            }
				
				CleanupExtraneousValues(GetAskVolumeForPrice[CurrentBar], lowPrice, tickSizeInterval);
	            CleanupExtraneousValues(GetBidVolumeForPrice[CurrentBar], lowPrice, tickSizeInterval);
	            CleanupExtraneousValues(GetDeltaForPrice[CurrentBar], lowPrice, tickSizeInterval);
	            CleanupExtraneousValues(GetTotalVolumeForPrice[CurrentBar], lowPrice, tickSizeInterval);
//				CleanupExtraneousValues(VP[startSessBarIndex], CurrentDayOHL().CurrentLow[0], tickSizeInterval);
							
				//PoC
				GetMaximumVolume[CurrentBar] = GetTotalVolumeForPrice[CurrentBar].Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
								
				// Track max bid and ask volumes for the current bar
				GetMaximumAskVolume[CurrentBar] = GetAskVolumeForPrice[CurrentBar].Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
				GetMaximumBidVolume[CurrentBar] = GetBidVolumeForPrice[CurrentBar].Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
				
				//Get max positive delta and max negative delta (idk if its ok this ;))
	            GetMaximumPositiveDelta[CurrentBar] = GetDeltaForPrice[CurrentBar].Max(kvp => kvp.Value);
	            GetMaximumNegativeDelta[CurrentBar] = GetDeltaForPrice[CurrentBar].Min(kvp => kvp.Value);
				
				//BigTrades
				foreach(var bt in GetTotalVolumeForPrice[CurrentBar])
				{
					if(bt.Value >= BTVol)
					{
						BigTrades[endSessBarIndex][CurrentBar] = bt.Key; //sessionEnd	
						BigTradesT[sessionEnd][CurrentBar] = bt.Key;
					}
				}
				
				CalculateImb();
				CalculateVA();
				CalculateRatios();
				FillVP();
			
			}
			catch(Exception ex)
			{
				Print(ex.ToString());
			}
			
		}
		
		private void CleanupExtraneousValues(Dictionary<double, double> dict, double lowPrice, double interval)
        {
            var keysToRemove = dict.Keys.Where(price => (price - lowPrice) % interval != 0).ToList();
            foreach (var key in keysToRemove)
            {
                dict.Remove(key);
            }
        }
		
		private void CalculateImb()
		{
			//Imbalances
//	        double offsetImb = MyImbType == ImbType.Diagonal ? TickSize*AggregationInterval : 0;	
			double offsetImb = TickSize*AggregationInterval;
			
	        foreach (var kvp in GetBidVolumeForPrice[CurrentBar])
	        {
	            var bidVolume = kvp.Value;
				
				//Diagonal
	            if (GetAskVolumeForPrice[CurrentBar].TryGetValue(kvp.Key + offsetImb, out var askVolumeD))
	            {
	                bidVolumeImbDictD[CurrentBar][kvp.Key] = bidVolume >= ImbFact && bidVolume >= ImbFact * askVolumeD && bidVolume >= MinVolH && askVolumeD >= MinVolL && askVolumeD <= MaxVolL;
	                askVolumeImbDictD[CurrentBar][kvp.Key + offsetImb] = askVolumeD >= ImbFact && ImbFact * bidVolume <= askVolumeD && askVolumeD >= MinVolH && bidVolume >= MinVolL && bidVolume <= MaxVolL;
	            }
				
				//Horizontal
				if (GetAskVolumeForPrice[CurrentBar].TryGetValue(kvp.Key, out var askVolumeH))
	            {
	                bidVolumeImbDictH[CurrentBar][kvp.Key] = bidVolume >= ImbFact && bidVolume >= ImbFact * askVolumeH && bidVolume >= MinVolH && askVolumeH >= MinVolL && askVolumeH <= MaxVolL;
	                askVolumeImbDictH[CurrentBar][kvp.Key] = askVolumeH >= ImbFact && ImbFact * bidVolume <= askVolumeH && askVolumeH >= MinVolH && bidVolume >= MinVolL && bidVolume <= MaxVolL;
	            }
	        }
						
			//Stacked imbalances
	        int consecutiveBidImbalancesD = 0;
	        int consecutiveAskImbalancesD = 0;	
	        int consecutiveBidImbalancesH = 0;
	        int consecutiveAskImbalancesH = 0;

			//Bid Diagonal
	        for (double price = Low[0]; price <= High[0] - TickSize*AggregationInterval; price += TickSize*AggregationInterval)
	        {
				if (bidVolumeImbDictD[CurrentBar].TryGetValue(price, out bool isBidImbalanceD) && isBidImbalanceD)
	                consecutiveBidImbalancesD++;
	            else
	                consecutiveBidImbalancesD = 0;
	
	            bidStckImbDictD[CurrentBar][price - TickSize*AggregationInterval*(StckImbNum - 1)] = consecutiveBidImbalancesD >= StckImbNum;
	        }

//			//Ask Diagonal
	        for (double price = Low[0] + TickSize*AggregationInterval; price <= High[0]; price += TickSize*AggregationInterval)
	        {
	            if (askVolumeImbDictD[CurrentBar].TryGetValue(price, out bool isAskImbalanceD) && isAskImbalanceD)
	                consecutiveAskImbalancesD++;
	            else
	                consecutiveAskImbalancesD = 0;
	
	            askStckImbDictD[CurrentBar][price] = consecutiveAskImbalancesD >= StckImbNum;
	        }
			
			//Bid Horizontal
	        for (double price = Low[0]; price <= High[0] - TickSize*AggregationInterval; price += TickSize*AggregationInterval)
	        {
				if (bidVolumeImbDictH[CurrentBar].TryGetValue(price, out bool isBidImbalanceH) && isBidImbalanceH)
	                consecutiveBidImbalancesH++;
	            else
	                consecutiveBidImbalancesH = 0;
	
	            bidStckImbDictH[CurrentBar][price - TickSize*AggregationInterval*(StckImbNum - 1)] = consecutiveBidImbalancesH >= StckImbNum;
	        }

//			//Ask Horizontal
	        for (double price = Low[0] + TickSize*AggregationInterval; price <= High[0]; price += TickSize*AggregationInterval)
	        {
	            if (askVolumeImbDictH[CurrentBar].TryGetValue(price, out bool isAskImbalanceH) && isAskImbalanceH)
	                consecutiveAskImbalancesH++;
	            else
	                consecutiveAskImbalancesH = 0;
	
	            askStckImbDictH[CurrentBar][price] = consecutiveAskImbalancesH >= StckImbNum;
	        }
		}
				
		private void CalculateVA()
		{
		    if (!GetTotalVolumeForPrice.ContainsKey(CurrentBar) || !GetMaximumVolume.ContainsKey(CurrentBar))
		        return;
		
		    // Calculate the target value area volume (e.g., 70% of the total volume)
		    double targetValueAreaVolume = barsType.Volumes[CurrentBar].TotalVolume * (ValueAreaPer / 100.0);
		
		    // Initialize PoC, cumulative volume, VAH, and VAL
		    double pocPrice = GetMaximumVolume[CurrentBar];
		    double cumulativeVolume = 0;
		    double vah = pocPrice;
		    double val = pocPrice;
		
		    // Get the list of sorted prices
		    var sortedPrices = GetTotalVolumeForPrice[CurrentBar].Keys.OrderBy(price => price).ToList();
		    int pocIndex = sortedPrices.IndexOf(pocPrice);
		
		    // Ensure the PoC exists in the list and prevent out-of-range access
		    if (pocIndex < 0 || sortedPrices.Count == 0)
		        return;
		
		    // Traverse up and down the price levels simultaneously
		    int upIndex = pocIndex + 1;
		    int downIndex = pocIndex - 1;
		
		    // Add PoC volume first
		    cumulativeVolume += GetTotalVolumeForPrice[CurrentBar][pocPrice];
		
		    while (cumulativeVolume < targetValueAreaVolume && (upIndex < sortedPrices.Count || downIndex >= 0))
		    {
		        double volumeUp = 0;
		        double volumeDown = 0;
		
		        if (upIndex < sortedPrices.Count && GetTotalVolumeForPrice[CurrentBar].TryGetValue(sortedPrices[upIndex], out volumeUp))
		        {
		            if (downIndex >= 0 && GetTotalVolumeForPrice[CurrentBar].TryGetValue(sortedPrices[downIndex], out volumeDown))
		            {
		                if (volumeUp >= volumeDown)
		                {
		                    cumulativeVolume += volumeUp;
		                    vah = sortedPrices[upIndex];
		                    upIndex++;
		                }
		                else
		                {
		                    cumulativeVolume += volumeDown;
		                    val = sortedPrices[downIndex];
		                    downIndex--;
		                }
		            }
		            else
		            {
		                cumulativeVolume += volumeUp;
		                vah = sortedPrices[upIndex];
		                upIndex++;
		            }
		        }
		        else if (downIndex >= 0 && GetTotalVolumeForPrice[CurrentBar].TryGetValue(sortedPrices[downIndex], out volumeDown))
		        {
		            cumulativeVolume += volumeDown;
		            val = sortedPrices[downIndex];
		            downIndex--;
		        }
		
		        // Stop if cumulative volume reaches or exceeds the target
		        if (cumulativeVolume >= targetValueAreaVolume)
		            break;
		    }
		
		    // Store the calculated VAH and VAL
		    VAH[CurrentBar] = vah;
		    VAL[CurrentBar] = val;
		}
		
		private void CalculateRatios()
		{
			double ratio = 0;

		    if (Close[0] > Open[0])
		    {
		        if (GetBidVolumeForPrice[CurrentBar].TryGetValue(Low[0] + TickSize * AggregationInterval, out double lowBidVolume) &&
		            GetBidVolumeForPrice[CurrentBar].TryGetValue(Low[0], out double lowVolume))
		        {
		            ratio = lowVolume > 0 ? lowBidVolume / (double)lowVolume : 0;
		        }
		    }
		    else if (Close[0] < Open[0])
		    {
				double maxAskPrice = GetAskVolumeForPrice[CurrentBar].Keys.Max();
		        if (GetAskVolumeForPrice[CurrentBar].TryGetValue(maxAskPrice - TickSize * AggregationInterval, out double highAskVolume) &&
		            GetAskVolumeForPrice[CurrentBar].TryGetValue(maxAskPrice, out double highVolume))
		        {
		            ratio = highVolume > 0 ? highAskVolume / (double)highVolume : 0;
		        }
		    }
		
		    BarRatio[CurrentBar] = ratio;
		}
				
		private void FillVP()
		{
			double tickSizeInterval = AggregationInterval * TickSize;
								
			for(int barIndex = startSessBarIndex; barIndex <= endSessBarIndex; barIndex++)
			{
				foreach (var vfp in GetTotalVolumeForPrice[barIndex])
				{
					if(!VP[startSessBarIndex].ContainsKey(vfp.Key))
						VP[startSessBarIndex][vfp.Key] = vfp.Value;
					else
						VP[startSessBarIndex][vfp.Key] += vfp.Value;
				}	
			}
			
			CleanupExtraneousValues(VP[startSessBarIndex], CurrentDayOHL().CurrentLow[0], tickSizeInterval);
			
			if (VP[startSessBarIndex].Count > 0)
				GetMaximumVolumeVP[startSessBarIndex] = VP[startSessBarIndex].Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
		}
		
		protected override void OnRender(ChartControl chartControl, ChartScale chartScale)
		{
		    base.OnRender(chartControl, chartScale);
			
			// Check if Bars are available and have enough data
		    if (Bars == null || Bars.Count == 0 || CurrentBar < BarsRequiredToPlot)
		        return;
		
		    // Ensure TickSize is valid
		    if (TickSize <= 0)
		        return;
		
		    ChartControlProperties myProperties = chartControl.Properties;
		    myProperties.BarMarginRight = 100;
		
		    if (myProperties.BarDistance <= BarWidth + 2 && !MinWidth)
		        myProperties.BarDistance = BarWidth + 2;
		
		    bool minBarWidth = MinWidth && myProperties.BarDistance <= BarWidth + 2;
		    double tickSizeInterval = TickSize * AggregationInterval;
			
		    try
		    {
		        for (int idx = ChartBars.FromIndex; idx <= ChartBars.ToIndex; idx++)
		        {
					// Ensure idx is within the valid range
		            if (idx < 0 || idx >= Bars.Count)
		                continue;
			
		            if (!GetBidVolumeForPrice.ContainsKey(idx) || !GetAskVolumeForPrice.ContainsKey(idx))
		                continue;
							
		            int x = chartControl.GetXByBarIndex(ChartBars, idx);
					int xLast = chartControl.GetXByBarIndex(ChartBars, ChartBars.ToIndex);
		            float xSeparator = myProperties.BarDistance;
										
		            var height = chartScale.GetYByValue(Open.GetValueAt(idx) + tickSizeInterval) - chartScale.GetYByValue(Open.GetValueAt(idx));
		            var offsetTick = AggregationInterval*TickSize / 2.5f;
					
//					Print(xSeparator + " " + (float)Font.Size*(xSeparator)/145 + " " + Math.Abs(height));
		
		            double o = Open.GetValueAt(idx);
		            double h = High.GetValueAt(idx);
		            double l = Low.GetValueAt(idx);
		            double c = Close.GetValueAt(idx);
					
		            float barOpen = chartScale.GetYByValue(o);
		            float barHigh = chartScale.GetYByValue(h);
		            float barLow = chartScale.GetYByValue(l);
		            float barClose = chartScale.GetYByValue(c);
		
		            var bidVol = GetBidVolumeForPrice[idx];
		            var askVol = GetAskVolumeForPrice[idx];
					
		            var bidImbD = bidVolumeImbDictD.ContainsKey(idx) ? bidVolumeImbDictD[idx] : new Dictionary<double, bool>();
		            var askImbD = askVolumeImbDictD.ContainsKey(idx) ? askVolumeImbDictD[idx] : new Dictionary<double, bool>();
					
					var bidImbH = bidVolumeImbDictH.ContainsKey(idx) ? bidVolumeImbDictH[idx] : new Dictionary<double, bool>();
		            var askImbH = askVolumeImbDictH.ContainsKey(idx) ? askVolumeImbDictH[idx] : new Dictionary<double, bool>();
					
		            var bidStckImbD = bidStckImbDictD.ContainsKey(idx) ? bidStckImbDictD[idx] : new Dictionary<double, bool>();
		            var askStckImbD = askStckImbDictD.ContainsKey(idx) ? askStckImbDictD[idx] : new Dictionary<double, bool>();
					
	           		var bidStckImbH = bidStckImbDictH.ContainsKey(idx) ? bidStckImbDictH[idx] : new Dictionary<double, bool>();
		            var askStckImbH = askStckImbDictH.ContainsKey(idx) ? askStckImbDictH[idx] : new Dictionary<double, bool>();
					
		            var vols = GetTotalVolumeForPrice[idx];
		            var deltas = GetDeltaForPrice[idx];
					
					var volProfile = VP.ContainsKey(idx) ? VP[idx] : new Dictionary<double, double>();
		
//		            double totalBid = TotalSellingVolume.ContainsKey(idx) ? TotalSellingVolume[idx] : 0;
//		            double totalAsk = TotalBuyingVolume.ContainsKey(idx) ? TotalBuyingVolume[idx] : 0;
		            double poc = GetMaximumVolume.ContainsKey(idx) ? GetMaximumVolume[idx] : 0;
		            double pocVol = GetTotalVolumeForPrice[idx].ContainsKey(poc) ? GetTotalVolumeForPrice[idx][poc] : 0;

		            double pocVP = GetMaximumVolumeVP.ContainsKey(idx) ? GetMaximumVolumeVP[idx] : 0;

					double pocVolVP =  volProfile.ContainsKey(pocVP) ? volProfile[pocVP] : 0;
					
		            double maxB = GetMaximumBidVolume.ContainsKey(idx) ? GetMaximumBidVolume[idx] : 0;
		            double maxA = GetMaximumAskVolume.ContainsKey(idx) ? GetMaximumAskVolume[idx] : 0;
		            double maxD = GetMaximumPositiveDelta.ContainsKey(idx) ? GetMaximumPositiveDelta[idx] : 0;
		            double minD = GetMaximumNegativeDelta.ContainsKey(idx) ? GetMaximumNegativeDelta[idx] : 0;
		            double cDelta = barsType.Volumes[idx].CumulativeDelta;
		            double deltaValue = barsType.Volumes[idx].BarDelta;
		            double volumeValue = barsType.Volumes[idx].TotalVolume;
		            double vah = VAH.ContainsKey(idx) ? VAH[idx] : 0;
		            double val = VAL.ContainsKey(idx) ? VAL[idx] : 0;
					double ratio = BarRatio.ContainsKey(idx) ? BarRatio[idx] : 0;
//					double bigTrades = BigTrades.ContainsKey(idx) ? BigTrades[idx] : 0;
					
					int hod  = 0;
					int lod  = 0;
				
					float askMargin = 0;
					float bidMargin = 0;
		
					//Body candlestick
					#region Body candlestick
					var BarWidthPaint = BarWidth % 2 == 0 ? BarWidth/2 : BarWidth/1.5f;
					var xOffset = x - (minBarWidth ? 1 : BarWidthPaint);
					//////////////////////////////////////////////////////////////////////////////////
					/// 
					
					//Upper wick				
					reuseVector.X		= x;
					reuseVector.Y		= chartScale.GetYByValue((c > o ? c : o) + offsetTick);
					reuseVector2.X		= reuseVector.X;
					reuseVector2.Y		= chartScale.GetYByValue(h + offsetTick);
					SharpDX.Direct2D1.Brush customDXBrushCS = (c > o ? BarUpColor : c < o ? BarDownColor : DojiColor).ToDxBrush(RenderTarget);
//					SharpDX.Direct2D1.Brush customDXBrushWC = WickColor.ToDxBrush(RenderTarget);
					RenderTarget.DrawLine(reuseVector, reuseVector2, customDXBrushCS, 1); 
				
					//Body
//					SharpDX.Direct2D1.Brush customDXBrushCS = (c > o ? BarUpColor : c < o ? BarDownColor : DojiColor).ToDxBrush(RenderTarget);
					reuseRect = new SharpDX.RectangleF(xOffset, chartScale.GetYByValue(o + (c < o ? 1 : -1) * offsetTick), minBarWidth ? 1 : BarWidth + (o == c ? 1 : 0), chartScale.GetYByValue(c + ((c < o ? -2 : 2)*offsetTick)) - barOpen);
					if(o == c)
//					if(o > c)
						RenderTarget.DrawRectangle(reuseRect, customDXBrushCS, 1);
					else
					{
						RenderTarget.FillRectangle(reuseRect, customDXBrushCS);
//						RenderTarget.DrawRectangle(reuseRect, customDXBrushWC, 1);
					}
				
					//Lower wick				
					reuseVector.X		= x;
					reuseVector.Y		= chartScale.GetYByValue((c < o ? c : o) - offsetTick);
					reuseVector2.X		= reuseVector.X;
					reuseVector2.Y		= chartScale.GetYByValue(l - offsetTick);
					RenderTarget.DrawLine(reuseVector, reuseVector2, customDXBrushCS, 1); 
					
					customDXBrushCS.Dispose();
//					customDXBrushWC.Dispose();
					#endregion
					
					///Render BID and ASK
					if(IsBidAsk)
		            {
		                foreach (var bid in bidVol)
		                {
		                    bool isBidImb = (ImbType ? bidImbD : bidImbH).ContainsKey(bid.Key) && (ImbType ? bidImbD : bidImbH)[bid.Key] && ShowImb;
		                    bool isBidStckImb = (ImbType ? bidStckImbD : bidStckImbH).ContainsKey(bid.Key) && (ImbType ? bidStckImbD : bidStckImbH)[bid.Key] && ShowStckImb;
		
		                    reuseTextFormat = new SharpDX.DirectWrite.TextFormat(Core.Globals.DirectWriteFactory, Font.Family.ToString(), SharpDX.DirectWrite.FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, xSeparator < 145 ? (float)Font.Size*(xSeparator)/145 : height > -(float)Font.Size ? Math.Abs(height) : (float)Font.Size);
		                    reuseTextFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Trailing;
		                    reuseTextLayout = new SharpDX.DirectWrite.TextLayout(Core.Globals.DirectWriteFactory, bid.Value.ToString(), reuseTextFormat, xSeparator / 4, (float)Font.Size);
							if(bid.Key == maxB)
								bidMargin = reuseTextLayout.Metrics.Width;
		                    reuseVector = new SharpDX.Vector2(x - BarWidth * 1.2f - reuseTextLayout.Metrics.LayoutWidth, chartScale.GetYByValue(bid.Key) - reuseTextLayout.Metrics.Height / 1.8f);
							SharpDX.Direct2D1.Brush customDXBrush = TextColor.ToDxBrush(RenderTarget);
		                    
							if(isBidStckImb || isBidImb)
							{
								reuseRect = new SharpDX.RectangleF(x - BarWidth * 1.2f + 2, chartScale.GetYByValue(bid.Key - offsetTick), -reuseTextLayout.Metrics.Width - 3, height * 0.8f);
								SharpDX.Direct2D1.Brush customDXBrushImb = isBidStckImb ? StckBidImbColor.ToDxBrush(RenderTarget) : BidImbColor.ToDxBrush(RenderTarget);
								RenderTarget.FillRectangle(reuseRect, customDXBrushImb);
								customDXBrushImb.Dispose();
							}
							
							if(bid.Key == poc && ShowTextB)
							{
								reuseRect = new SharpDX.RectangleF(x - BarWidth * 1.2f + 2, chartScale.GetYByValue(bid.Key - offsetTick), -reuseTextLayout.Metrics.Width - 3, height * 0.8f);
								SharpDX.Direct2D1.Brush customDXBrushPoc = PocColor.ToDxBrush(RenderTarget);
								RenderTarget.DrawRectangle(reuseRect, customDXBrushPoc, 1);
								customDXBrushPoc.Dispose();
							}
							
//							//Render text here
							if(ShowTextB)
								RenderTarget.DrawTextLayout(reuseVector, reuseTextLayout, customDXBrush);
							
		                    customDXBrush.Dispose();
		                    reuseTextFormat.Dispose();
		                    reuseTextLayout.Dispose();
		                }
		
		                foreach (var ask in askVol)
		                {
		                    bool isAskImb = (ImbType ? askImbD : askImbH).ContainsKey(ask.Key) && (ImbType ? askImbD : askImbH)[ask.Key] && ShowImb;
		                    bool isAskStckImb = (ImbType ? askStckImbD : askStckImbH).ContainsKey(ask.Key) && (ImbType ? askStckImbD : askStckImbH)[ask.Key] && ShowStckImb;
		
		                    reuseTextFormat = new SharpDX.DirectWrite.TextFormat(Core.Globals.DirectWriteFactory, Font.Family.ToString(), SharpDX.DirectWrite.FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, xSeparator < 145 ? (float)Font.Size*(xSeparator)/145 : height > -(float)Font.Size ? Math.Abs(height) : (float)Font.Size);
		                    reuseTextFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading;
		                    reuseTextLayout = new SharpDX.DirectWrite.TextLayout(Core.Globals.DirectWriteFactory, ask.Value.ToString(), reuseTextFormat, xSeparator / 4, (float)Font.Size);
							if(ask.Key == maxA)
								askMargin = reuseTextLayout.Metrics.Width;
		                    reuseVector = new SharpDX.Vector2(x + BarWidth, chartScale.GetYByValue(ask.Key) - reuseTextLayout.Metrics.Height / 1.8f);
//		                    SharpDX.Direct2D1.Brush customDXBrush = isAskStckImb ? StckAskImbColor.ToDxBrush(RenderTarget) : isAskImb ? AskImbColor.ToDxBrush(RenderTarget) : TextColor.ToDxBrush(RenderTarget);
							SharpDX.Direct2D1.Brush customDXBrush = TextColor.ToDxBrush(RenderTarget);
		                    
							if(isAskStckImb || isAskImb)
							{
								reuseRect = new SharpDX.RectangleF(x + BarWidth * 1.2f - 2, chartScale.GetYByValue(ask.Key - offsetTick), reuseTextLayout.Metrics.Width + 3, height * 0.8f);
								SharpDX.Direct2D1.Brush customDXBrushImb = isAskStckImb ? StckAskImbColor.ToDxBrush(RenderTarget) : AskImbColor.ToDxBrush(RenderTarget);
								RenderTarget.FillRectangle(reuseRect, customDXBrushImb);
								customDXBrushImb.Dispose();
							}
							
							if(ask.Key == poc && ShowTextA)
							{
								reuseRect = new SharpDX.RectangleF(x + BarWidth * 1.2f - 2, chartScale.GetYByValue(ask.Key - offsetTick), reuseTextLayout.Metrics.Width + 3, height * 0.8f);
								SharpDX.Direct2D1.Brush customDXBrushPoc = PocColor.ToDxBrush(RenderTarget);
								RenderTarget.DrawRectangle(reuseRect, customDXBrushPoc, 1);
								customDXBrushPoc.Dispose();
							}
		
							//Render text here
							if(ShowTextA)
								RenderTarget.DrawTextLayout(reuseVector, reuseTextLayout, customDXBrush);
							
		                    customDXBrush.Dispose();
		                    reuseTextFormat.Dispose();
		                    reuseTextLayout.Dispose();
		                }	
						
						//VA Square	OR Line	
						if(ShowVA)
						{
							//Square
	//						float vaEight = chartScale.GetYByValue(vah) - (chartScale.GetYByValue(val) - height);
	//						reuseRect = new SharpDX.RectangleF(x - BarWidth * 1.2f - (float)bidMargin - 3, chartScale.GetYByValue(val - AggregationInterval*TickSize/2.325f), (float)askMargin + (float)bidMargin + BarWidth * 1.2f + 12, vaEight );
	//						SharpDX.Direct2D1.Brush customDXBrushVA = (c > o ? BarUpColor : c < o ? BarDownColor : DojiColor).ToDxBrush(RenderTarget);
	//						customDXBrushVA.Opacity = 0.25f;
	//						RenderTarget.DrawRectangle(reuseRect, customDXBrushVA, 1);
	//						customDXBrushVA.Dispose();
							
							//Line right
							//System.Windows.Media.Brush
							System.Windows.Media.Brush  color = (c > o ? BarUpColor : c < o ? BarDownColor : DojiColor);
							
							//Right bracket
							VABrackets(x + askMargin + BarWidth * 2, chartScale.GetYByValue(val - AggregationInterval*TickSize/2.325f), 0, chartScale.GetYByValue(vah) + height*0.6f, color, true);
							VABrackets(x + askMargin + BarWidth * 2, chartScale.GetYByValue(vah) + height*0.6f, reuseVector.X - 4, 0, color, false);
							VABrackets(x + askMargin + BarWidth * 2, chartScale.GetYByValue(val - AggregationInterval*TickSize/2.325f), reuseVector.X - 4, 0, color, false);
							
							//Left bracket
							VABrackets(x - bidMargin - BarWidth * 2, chartScale.GetYByValue(val - AggregationInterval*TickSize/2.325f), 0, chartScale.GetYByValue(vah) + height*0.6f, color, true);
							VABrackets(x - bidMargin - BarWidth * 2, chartScale.GetYByValue(vah) + height*0.6f, reuseVector.X + 4, 0, color, false);
							VABrackets(x - bidMargin - BarWidth * 2, chartScale.GetYByValue(val - AggregationInterval*TickSize/2.325f), reuseVector.X + 4, 0, color, false);
						}
		            }
					
					else
					///Render Volume histograms
					{
						//Histogram volume
						if(ShowHistVol)
						{
			                foreach (var vol in vols)
			                {
			                    if (pocVol == 0) continue; // Prevent division by zero
								
								//VA	
								if(ShowVA)
								{
									float vaWidth = (chartScale.GetYByValue(vah - AggregationInterval*TickSize/2.325f)) - (chartScale.GetYByValue(val - AggregationInterval*TickSize/2.325f) - height * 0.8f);
									reuseRect = new SharpDX.RectangleF(x + BarWidth*1.5f, chartScale.GetYByValue(val - AggregationInterval*TickSize/2.325f), 0.25f * xSeparator + 5, vaWidth );
									SharpDX.Direct2D1.Brush customDXBrushVA = VAColor.ToDxBrush(RenderTarget);
//									SharpDX.Direct2D1.Brush customDXBrushVA = (c > o ? BarUpColor : c < o ? BarDownColor : DojiColor).ToDxBrush(RenderTarget);
		
									RenderTarget.DrawRectangle(reuseRect, customDXBrushVA, 1);
									
				                    customDXBrushVA.Dispose();	
								}
								
								//Imbalances
								bool isBidImb = (ImbType ? bidImbD : bidImbH).ContainsKey(vol.Key) && (ImbType ? bidImbD : bidImbH)[vol.Key] && ShowImb;
			                    bool isBidStckImb = (ImbType ? bidStckImbD : bidStckImbH).ContainsKey(vol.Key) && (ImbType ? bidStckImbD : bidStckImbH)[vol.Key] && ShowStckImb;
			                    bool isAskImb = (ImbType ? askImbD : askImbH).ContainsKey(vol.Key) && (ImbType ? askImbD : askImbH)[vol.Key] && ShowImb;
			                    bool isAskStckImb = (ImbType ? askStckImbD : askStckImbH).ContainsKey(vol.Key) && (ImbType ? askStckImbD : askStckImbH)[vol.Key] && ShowStckImb;
			
			                    var width = (0.25f * xSeparator * vol.Value) / (float)(pocVol) <= 2 ? 2 : (0.25f * xSeparator * vol.Value) / (float)(pocVol);
								reuseRect = new SharpDX.RectangleF(x + BarWidth*1.5f, chartScale.GetYByValue(vol.Key - offsetTick), (float)width, height * 0.8f);
							
								
//								bool typeImb = !ImbType;
//								SharpDX.Direct2D1.Brush customDXBrushVolH = 
//								 typeImb && isBidStckImb ? StckBidImbColor.ToDxBrush(RenderTarget) : 
//								 typeImb && isAskStckImb ? StckAskImbColor.ToDxBrush(RenderTarget) : 
//								 typeImb && isBidImb ? BidImbColor.ToDxBrush(RenderTarget) : 
//								 typeImb && isAskImb ? AskImbColor.ToDxBrush(RenderTarget) : 
//								 ShowVA && (vol.Key >= val && vol.Key <= vah) ? VAColor.ToDxBrush(RenderTarget) :
//								 VolColor.ToDxBrush(RenderTarget);
								

								SharpDX.Direct2D1.Brush customDXBrushVolH = 
								 isBidStckImb ? StckBidImbColor.ToDxBrush(RenderTarget) : 
								 isAskStckImb ? StckAskImbColor.ToDxBrush(RenderTarget) : 
								 isBidImb ? BidImbColor.ToDxBrush(RenderTarget) : 
								 isAskImb ? AskImbColor.ToDxBrush(RenderTarget) : 
								 ShowVA && (vol.Key >= val && vol.Key <= vah) ? VAColor.ToDxBrush(RenderTarget) :
								 VolColor.ToDxBrush(RenderTarget);
								
								if(ShowTextV)
									customDXBrushVolH.Opacity = 0.5f;
									
								RenderTarget.FillRectangle(reuseRect, customDXBrushVolH);
			                    customDXBrushVolH.Dispose();
								
								//POC
								if(vol.Key == poc)
								{
									reuseRect.Width -= 2; 
									reuseRect.Height += 2; 
									reuseRect.X += 1;
									reuseRect.Y -= 1;
									SharpDX.Direct2D1.Brush customDXBrushPoc = PocColor.ToDxBrush(RenderTarget);
									RenderTarget.DrawRectangle(reuseRect, customDXBrushPoc, 2);
									customDXBrushPoc.Dispose();
								}								
			                }
						}
						
						//Volume values
						if(ShowTextV)
						{
							foreach (var v in vols)
			                {
			                    reuseTextFormat = new SharpDX.DirectWrite.TextFormat(Core.Globals.DirectWriteFactory, Font.Family.ToString(), SharpDX.DirectWrite.FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, xSeparator < 145 ? (float)Font.Size*(xSeparator)/145 : height > -(float)Font.Size ? Math.Abs(height) : (float)Font.Size);
			                    reuseTextFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading;
			                    reuseTextLayout = new SharpDX.DirectWrite.TextLayout(Core.Globals.DirectWriteFactory, v.Value.ToString(), reuseTextFormat, xSeparator / 2, (float)Font.Size);
			                    reuseVector = new SharpDX.Vector2(x + BarWidth*1.8f, chartScale.GetYByValue(v.Key) - reuseTextLayout.Metrics.Height / 1.8f);
			                    SharpDX.Direct2D1.Brush customDXBrush = TextColor.ToDxBrush(RenderTarget);
			                    RenderTarget.DrawTextLayout(reuseVector, reuseTextLayout, customDXBrush);
			
			                    customDXBrush.Dispose();
			                    reuseTextFormat.Dispose();
			                    reuseTextLayout.Dispose();
							}
						}
						
						//Histogram delta
						if(ShowHistDelta)
						{
			                foreach (var delta in deltas)
			                {
			                    if (Math.Max(Math.Abs(maxD), Math.Abs(minD)) == 0) continue; // Prevent division by zero
			
			                    var width = (0.25f * xSeparator * delta.Value) / (float)(Math.Max(Math.Abs(maxD), Math.Abs(minD))) <= 1 ? 1 : (0.25f * xSeparator * delta.Value) / (float)(Math.Max(Math.Abs(maxD), Math.Abs(minD)));
			                    reuseRect = new SharpDX.RectangleF(x - BarWidth*1.5f, chartScale.GetYByValue(delta.Key - offsetTick), -(float)(0.25f * xSeparator * Math.Abs(delta.Value)) / (float)(Math.Max(Math.Abs(maxD), Math.Abs(minD))), (float)height * 0.8f);
			                    SharpDX.Direct2D1.Brush customDXBrushDeltaPos = DeltaPosColor.ToDxBrush(RenderTarget);
			                    SharpDX.Direct2D1.Brush customDXBrushDeltaNeg = DeltaNegColor.ToDxBrush(RenderTarget);
								if(ShowTextD)
								{
									customDXBrushDeltaPos.Opacity = 0.5f;
									customDXBrushDeltaNeg.Opacity = 0.5f;
								}
			                    RenderTarget.FillRectangle(reuseRect, delta.Value > 0 ? customDXBrushDeltaPos : customDXBrushDeltaNeg);
			                    customDXBrushDeltaPos.Dispose();
			                    customDXBrushDeltaNeg.Dispose();
			                }
						}
									
						//Delta values
						if(ShowTextD)
						{
			                foreach (var d in deltas)
			                {
			                    reuseTextFormat = new SharpDX.DirectWrite.TextFormat(Core.Globals.DirectWriteFactory, Font.Family.ToString(), SharpDX.DirectWrite.FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, xSeparator < 145 ? (float)Font.Size*(xSeparator)/145 : height > -(float)Font.Size ? Math.Abs(height) : (float)Font.Size);
			                    reuseTextFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Trailing;
			                    reuseTextLayout = new SharpDX.DirectWrite.TextLayout(Core.Globals.DirectWriteFactory, d.Value.ToString(), reuseTextFormat, xSeparator / 2, (float)Font.Size);
			                    reuseVector = new SharpDX.Vector2(x - BarWidth*1.8f - reuseTextLayout.Metrics.LayoutWidth, chartScale.GetYByValue(d.Key) - reuseTextLayout.Metrics.Height / 1.8f);
			                    SharpDX.Direct2D1.Brush customDXBrushD = TextColor.ToDxBrush(RenderTarget);
			                    RenderTarget.DrawTextLayout(reuseVector, reuseTextLayout, customDXBrushD);
			
			                    customDXBrushD.Dispose();
			                    reuseTextFormat.Dispose();
			                    reuseTextLayout.Dispose();
			                }
						}
					}
					
//					//First bar of the session
					
//					if(firstBar)
//					{
//						reuseTextFormat = new SharpDX.DirectWrite.TextFormat(Core.Globals.DirectWriteFactory, Font.Family.ToString(), SharpDX.DirectWrite.FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, (float)Font.Size);
//						reuseTextLayout	= new SharpDX.DirectWrite.TextLayout(Core.Globals.DirectWriteFactory, "▼", reuseTextFormat, xSeparator, (float)Font.Size*1.2f);
//						reuseVector = new SharpDX.Vector2(x - reuseTextLayout.Metrics.Width/2, ChartPanel.Y + 15 - (float)Font.Size);							
//						SharpDX.Direct2D1.Brush customDXBrushR =  UnFinishAuctColor.ToDxBrush(RenderTarget);
//						RenderTarget.DrawTextLayout(reuseVector, reuseTextLayout, customDXBrushR);
						
//						customDXBrushR.Dispose();
//						reuseTextFormat.Dispose();
//						reuseTextLayout.Dispose();
//					}
					
					///Ratios
					if(ShowRatios)
					{
						reuseTextFormat = new SharpDX.DirectWrite.TextFormat(Core.Globals.DirectWriteFactory, Font.Family.ToString(), SharpDX.DirectWrite.FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, (float)Font.Size);
						reuseTextLayout	= new SharpDX.DirectWrite.TextLayout(Core.Globals.DirectWriteFactory, 
						 (c > o) && (ratio >= RatioH) ? "△ " + ratio.ToString("F2") :
						 (c > o && ratio <= RatioL && ratio > 0) ? "▲ " + ratio.ToString("F2") :  
						 (c < o && ratio >= RatioH) ? "▽ " + ratio.ToString("F2") :
						 (c < o && ratio <= RatioL && ratio > 0) ? "▼ " + ratio.ToString("F2") : 
						 ratio.ToString("F2"), reuseTextFormat, xSeparator, (float)Font.Size*1.2f);
						reuseVector = new SharpDX.Vector2(x - reuseTextLayout.Metrics.Width/2, c > o  ? (float)barLow - height - (float)Font.Size/2 : (float)barHigh + height - (float)Font.Size);							
						SharpDX.Direct2D1.Brush customDXBrushR =  c > o && (ratio >= RatioH || ratio <= RatioL) && ratio > 0 ? BarUpColor.ToDxBrush(RenderTarget) : c < o && (ratio <= RatioL || ratio >= RatioH) && ratio > 0 ? BarDownColor.ToDxBrush(RenderTarget) : TextColor.ToDxBrush(RenderTarget);
						RenderTarget.DrawTextLayout(reuseVector, reuseTextLayout, customDXBrushR);
						
						customDXBrushR.Dispose();
						reuseTextFormat.Dispose();
						reuseTextLayout.Dispose();
					}
					
					///LAB Absorption
					if (((deltaValue > 0 && c < o ) || (deltaValue < 0 && c > o )) && ShowABS)
					{
						bool bigAbsorption = Math.Abs(deltaValue) >= Math.Abs(deltaAv);
						reuseTextFormat = new SharpDX.DirectWrite.TextFormat(Core.Globals.DirectWriteFactory, Font.Family.ToString(), SharpDX.DirectWrite.FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, (float)Font.Size);
//						reuseTextLayout	= new SharpDX.DirectWrite.TextLayout(Core.Globals.DirectWriteFactory, (bigAbsorption ? "🅱" : "") + "🅰️\n" + Math.Abs(deltaValue).ToString(), reuseTextFormat, xSeparator, (float)Font.Size*1.2f); //🧽
						reuseTextLayout	= new SharpDX.DirectWrite.TextLayout(Core.Globals.DirectWriteFactory, (bigAbsorption && c > o ? "⏬\n" : bigAbsorption && c < o  ? "⏫\n" : "🅰️\n") + (xSeparator >= 10 ? Math.Abs(deltaValue).ToString() : ""), reuseTextFormat, xSeparator*3, (float)Font.Size*1.2f); //🧽
//						reuseTextFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
						reuseVector = new SharpDX.Vector2(x - reuseTextLayout.Metrics.Width/2, c > o  ? (float)barLow - height*6 - (float)Font.Size/2 : (float)barHigh + height*4 - (float)Font.Size);							
						SharpDX.Direct2D1.Brush customDXBrushAbs = (bigAbsorption && c > o ? AbsorptionBidColor : bigAbsorption && c < o ? AbsorptionAskColor : c > o ? BarDownColor : BarUpColor).ToDxBrush(RenderTarget);
//						SharpDX.Direct2D1.Brush customDXBrushAbs = (c > o ? BarDownColor : BarUpColor).ToDxBrush(RenderTarget);

						RenderTarget.DrawTextLayout(reuseVector, reuseTextLayout, customDXBrushAbs);
						
						customDXBrushAbs.Dispose();
						reuseTextFormat.Dispose();
						reuseTextLayout.Dispose();
							
					}
					
					///HLOD
					if(ShowHLOD)
					{
						for (int session = 1; session <= sessionCounter; session++)
						{	
							hod = HOD.ContainsKey(session) ? HOD[session] : 0;
							lod = LOD.ContainsKey(session) ? LOD[session] : 0;
							
							bool isHOD = idx == hod;
							bool isLOD = idx == lod;
							bool isHODDiv = isHOD && deltaValue < 0;
							bool isLODDiv = isLOD && deltaValue > 0;
							
							reuseTextFormat = new SharpDX.DirectWrite.TextFormat(Core.Globals.DirectWriteFactory, Font.Family.ToString(), SharpDX.DirectWrite.FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, (float)Font.Size);
							reuseTextLayout	= new SharpDX.DirectWrite.TextLayout(Core.Globals.DirectWriteFactory, isLODDiv ? "🔽" : isLOD ? "▼" : isHODDiv ? "🔼" : isHOD? "▲" : "", reuseTextFormat, xSeparator, (float)Font.Size*1.2f);
							reuseVector = new SharpDX.Vector2(x - reuseTextLayout.Metrics.Width/2, idx == lod  ? (float)barLow - height - (float)Font.Size/2 : idx == hod  ? (float)barHigh + height - (float)Font.Size : 0);
//							SharpDX.Direct2D1.Brush customDXBrushHLD = (isHODDiv || isLODDiv ? UnFinishAuctColor : isHOD ? BarUpColor : BarDownColor).ToDxBrush(RenderTarget);
							SharpDX.Direct2D1.Brush customDXBrushHLD = UnFinishAuctColor.ToDxBrush(RenderTarget);
							RenderTarget.DrawTextLayout(reuseVector, reuseTextLayout, customDXBrushHLD);
							
							customDXBrushHLD.Dispose();
							reuseTextFormat.Dispose();
							reuseTextLayout.Dispose();
						}
					}
					
					
					///BIG Trades
//					if(ShowBT)
//					{
						foreach(var v in vols)
							if(v.Value >= BTVol)
							{
								reuseVector.X	= x;
								reuseVector.Y	= chartScale.GetYByValue(v.Key);
								reuseVector2.X	= x;
								reuseVector2.Y	= chartScale.GetYByValue(v.Key);
								
								SharpDX.Vector2 centerPoint = (reuseVector + reuseVector2) / 2;
								
								SharpDX.Direct2D1.Brush customDXBrushBT = (deltas[v.Key] > 0 ? BarUpColor : BarDownColor) .ToDxBrush(RenderTarget); 
								
								if(ShowBTCircles)
								{
									customDXBrushBT.Opacity = 0.15f;
									SharpDX.Direct2D1.Ellipse ellipse = new SharpDX.Direct2D1.Ellipse(centerPoint, (float)(v.Value)*20/BTVol, (float)(v.Value)*20/BTVol);
									RenderTarget.FillEllipse(ellipse, customDXBrushBT);
									customDXBrushBT.Opacity = 1;
									RenderTarget.DrawEllipse(ellipse, customDXBrushBT);
								}

								if(ShowBTLSR && v.Value >= BTVol*BTVolLVLSR && v.Value <= BTVol*BTVolHVLSR)
									

								{
									reuseVector2.X 	= chartControl.GetXByBarIndex(ChartBars, ChartBars.ToIndex);
									RenderTarget.DrawLine(reuseVector, reuseVector2, customDXBrushBT);
									
									//Text
									if(BTLSRText)
									{
										int lastIdx = chartControl.GetXByBarIndex(ChartBars, ChartBars.ToIndex);
										double btLevel = chartScale.GetYByValue(v.Key);
										reuseTextFormat = new SharpDX.DirectWrite.TextFormat(Core.Globals.DirectWriteFactory, Font.Family.ToString(), SharpDX.DirectWrite.FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, (float)Font.Size);
										reuseTextLayout	= new SharpDX.DirectWrite.TextLayout(Core.Globals.DirectWriteFactory, v.Value.ToString() + "/" + v.Key.ToString() , reuseTextFormat, 100, (float)Font.Size*1.2f);
										reuseVector = new SharpDX.Vector2(lastIdx + 3, (float)btLevel - (float)Font.Size/1.5f);
			//							SharpDX.Direct2D1.Brush customDXBrushHLD = (isHODDiv || isLODDiv ? UnFinishAuctColor : isHOD ? BarUpColor : BarDownColor).ToDxBrush(RenderTarget);
	//									SharpDX.Direct2D1.Brush customDXBrushHLD = UnFinishAuctColor.ToDxBrush(RenderTarget);
										RenderTarget.DrawTextLayout(reuseVector, reuseTextLayout, customDXBrushBT);
	
										reuseTextFormat.Dispose();
										reuseTextLayout.Dispose();
									}
									
								}
								
								customDXBrushBT.Dispose();	//Moved to down to render BT lines
							}
//					}
					
					///Summary table
					if(ShowSummTab)
					{
						//Celdas para valores
						float eachXCell = x - xSeparator/2 + 4;
						float eachYCell = ChartPanel.Y + ChartPanel.H;
						float eachWidthCell = xSeparator - 4;
						float eachHeightCell = 20;
						float eachXFixedCell = ChartPanel.X + ChartPanel.W - myProperties.BarMarginRight*0.7f;
						float eachWidthFixedCell = myProperties.BarMarginRight*0.7f;
						float eachHeightFixedCell = 20;
						float eachWidthFixedText = ChartPanel.X + ChartPanel.W - myProperties.BarMarginRight*0.7f + 4;
						
						bool div = (deltaValue > 0 && o < c ) || (deltaValue < 0 && o > c ) ? true : false;
						
//						SummaryTable(eachXCell, eachYCell - 176, eachWidthCell, eachHeightCell, Brushes.Transparent.ToDxBrush(RenderTarget), div ? 0 : 1, x, xSeparator, 20, eachYCell - 173, eachXFixedCell, eachYCell - 176, eachWidthFixedCell, eachHeightFixedCell, "Signal", eachWidthFixedText, div);
						SummaryTable(eachXCell, eachYCell - 154, eachWidthCell, eachHeightCell, deltaValue > 0 ? DeltaPosColor.ToDxBrush(RenderTarget) : deltaValue < 0 ? DeltaNegColor.ToDxBrush(RenderTarget) : PocColor.ToDxBrush(RenderTarget), deltaValue, x, xSeparator, 20, eachYCell - 151, eachXFixedCell, eachYCell - 154, eachWidthFixedCell, eachHeightFixedCell, "Delta", eachWidthFixedText, false);
						SummaryTable(eachXCell, eachYCell - 132, eachWidthCell, eachHeightCell, maxD == 0 ? PocColor.ToDxBrush(RenderTarget) : DeltaPosColor.ToDxBrush(RenderTarget), maxD, x, xSeparator, 20, eachYCell - 129, eachXFixedCell, eachYCell - 132, eachWidthFixedCell, eachHeightFixedCell, "MaxDelta", eachWidthFixedText, false);
						SummaryTable(eachXCell, eachYCell - 110, eachWidthCell, eachHeightCell, minD == 0 ? PocColor.ToDxBrush(RenderTarget) : DeltaNegColor.ToDxBrush(RenderTarget), minD, x, xSeparator, 20, eachYCell - 107, eachXFixedCell, eachYCell - 110, eachWidthFixedCell, eachHeightFixedCell, "MinDelta", eachWidthFixedText, false);
						SummaryTable(eachXCell, eachYCell - 88, eachWidthCell, eachHeightCell, cDelta > 0 ? DeltaPosColor.ToDxBrush(RenderTarget) : cDelta < 0 ? DeltaNegColor.ToDxBrush(RenderTarget) : PocColor.ToDxBrush(RenderTarget), cDelta, x, xSeparator, 20, eachYCell - 85, eachXFixedCell, eachYCell - 88, eachWidthFixedCell, eachHeightFixedCell, "CumDelta", eachWidthFixedText, false);
						SummaryTable(eachXCell, eachYCell - 66, eachWidthCell, eachHeightCell, deltaValue/volumeValue > 0 ? DeltaPosColor.ToDxBrush(RenderTarget) : deltaValue/volumeValue < 0 ? DeltaNegColor.ToDxBrush(RenderTarget) : VolColor.ToDxBrush(RenderTarget), Math.Round(100*deltaValue/volumeValue, 2), x, xSeparator, 20, eachYCell - 63, eachXFixedCell, eachYCell - 66, eachWidthFixedCell, eachHeightFixedCell, "Delta/Vol", eachWidthFixedText, false);
						SummaryTable(eachXCell, eachYCell - 44, eachWidthCell, eachHeightCell, volumeValue >= BigBarVol ? Brushes.Purple.ToDxBrush(RenderTarget) : VolColor.ToDxBrush(RenderTarget), volumeValue, x, xSeparator, 20, eachYCell - 41, eachXFixedCell, eachYCell - 44, eachWidthFixedCell, eachHeightFixedCell, "Volume", eachWidthFixedText, false);
					}
															
					///VP
					if(ShowVP)
					{
						foreach (var vp in volProfile)
		                {
							if (pocVolVP == 0) continue;
							var width = (VPWidth * xSeparator * vp.Value) / (float)(pocVolVP) <= 5 ? 5 : (VPWidth * xSeparator * vp.Value) / (float)(pocVolVP);
	//						reuseRect = new SharpDX.RectangleF(ChartPanel.X + ChartPanel.W - myProperties.BarMarginRight*0.7f + 4, chartScale.GetYByValue(vp.Key - AggregationInterval*TickSize/2.325f), -(float)width, height * 0.8f);
							reuseRect = new SharpDX.RectangleF(x, chartScale.GetYByValue(vp.Key - AggregationInterval*TickSize/2.325f), (float)width, (float)height * 0.8f);
							SharpDX.Direct2D1.Brush customDXBrushVP = vp.Key == pocVP ? PocColor.ToDxBrush(RenderTarget) : VPColor.ToDxBrush(RenderTarget);
							customDXBrushVP.Opacity = 0.2f;
	//						RenderTarget.DrawRectangle(reuseRect, customDXBrushVP, 1);
							RenderTarget.FillRectangle(reuseRect, customDXBrushVP);
		                    customDXBrushVP.Dispose();	
							
							//Text
//							reuseTextFormat = new SharpDX.DirectWrite.TextFormat(Core.Globals.DirectWriteFactory, Font.Family.ToString(), (float)Font.Size);
//		                    reuseTextFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Trailing;
//		                    reuseTextLayout = new SharpDX.DirectWrite.TextLayout(Core.Globals.DirectWriteFactory, (vp.Value/1000).ToString(), reuseTextFormat, xSeparator*4, (float)Font.Size);
//		                    reuseVector = new SharpDX.Vector2(x - BarWidth*1.8f - reuseTextLayout.Metrics.LayoutWidth, chartScale.GetYByValue(vp.Key) - reuseTextLayout.Metrics.Height / 1.8f);
//		                    SharpDX.Direct2D1.Brush customDXBrushVPText = TextColor.ToDxBrush(RenderTarget);
//		                    RenderTarget.DrawTextLayout(reuseVector, reuseTextLayout, customDXBrushVPText);
	
//		                    customDXBrushVPText.Dispose();
//		                    reuseTextFormat.Dispose();
//		                    reuseTextLayout.Dispose();
						}
					}		

				
//				//Params info chart
//				if(ShowParInfo)
//				{
//					reuseRect = new SharpDX.RectangleF(ChartPanel.X + ChartPanel.W - myProperties.BarMarginRight*0.7f - 10, ChartPanel.Y + 110, myProperties.BarMarginRight*0.7f, 20);
//					RenderTarget.FillRectangle(reuseRect, VolColor.ToDxBrush(RenderTarget));
//					reuseRect = new SharpDX.RectangleF(ChartPanel.X + ChartPanel.W - myProperties.BarMarginRight*0.7f - 10, ChartPanel.Y + 88, myProperties.BarMarginRight*0.7f, 20);
//					RenderTarget.FillRectangle(reuseRect, VolColor.ToDxBrush(RenderTarget));
//					reuseRect = new SharpDX.RectangleF(ChartPanel.X + ChartPanel.W - myProperties.BarMarginRight*0.7f - 10, ChartPanel.Y + 66, myProperties.BarMarginRight*0.7f, 20);
//					RenderTarget.FillRectangle(reuseRect, VolColor.ToDxBrush(RenderTarget));
//					reuseRect = new SharpDX.RectangleF(ChartPanel.X + ChartPanel.W - myProperties.BarMarginRight*0.7f - 10, ChartPanel.Y + 44, myProperties.BarMarginRight*0.7f, 20);
//					RenderTarget.FillRectangle(reuseRect, VolColor.ToDxBrush(RenderTarget));
//				}	
					
				}//End of For Loop
				

				///Valores Debug
				reuseTextFormat = new SharpDX.DirectWrite.TextFormat(Core.Globals.DirectWriteFactory, Font.Family.ToString(), SharpDX.DirectWrite.FontWeight.DemiBold, SharpDX.DirectWrite.FontStyle.Normal, 12);
				reuseTextFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Trailing;
				reuseTextLayout = new SharpDX.DirectWrite.TextLayout(Core.Globals.DirectWriteFactory, "Aggregation: " + AggregationInterval.ToString() +
				 /*", ImbFact: " + ImbFact.ToString() + ", TypeImb: " + (MyImbType == ImbType.Diagonal ? "Diagonal" : "Horizontal") + */
				 ", ImbFact: " + ImbFact.ToString() + ", TypeImb: " + (ImbType ? "Diagonal" : "Horizontal") +
				 ", StckImbNum: " + StckImbNum.ToString() + ", DeltaAv: " + deltaAv.ToString("F2"), reuseTextFormat, myProperties.BarMarginRight*5, 20);
				reuseVector = new SharpDX.Vector2(ChartPanel.X + ChartPanel.W - myProperties.BarMarginRight*6, ChartPanel.Y + ChartPanel.H - 18);
				RenderTarget.DrawTextLayout(reuseVector, reuseTextLayout, TextColor.ToDxBrush(RenderTarget));
				
				reuseTextFormat.Dispose();
				reuseTextLayout.Dispose();			
		    }
		    catch (Exception exception)
		    {
		        Print(exception);
		    }
		}
		
		public override string DisplayName => Name;
		
		private void SummaryTable(float eachXCell, float eachYCell, float eachWidthCell, float eachHeightCell, SharpDX.Direct2D1.Brush eachColor, double eachCellValue, float x, float eachTextWidth, float eachTextHeight, float eachTextYPanel, float eachXFixedCell, float eachYFixedCell, float eachWidthFixedCell, float eachHeightFixedCell, string cellName, float eachWidthFixedText, bool signal)
		{
			//Celdas para valores
			reuseRect = new SharpDX.RectangleF(eachXCell, eachYCell, eachWidthCell, eachHeightCell);
			RenderTarget.FillRectangle(reuseRect, eachColor);
			
			//Texto de valores de cada barra en el sumario
			reuseTextFormat = new SharpDX.DirectWrite.TextFormat(Core.Globals.DirectWriteFactory, Font.Family.ToString(), SharpDX.DirectWrite.FontWeight.Bold, SharpDX.DirectWrite.FontStyle.Normal, (float)Font.Size);
			reuseTextFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
			reuseTextLayout	= new SharpDX.DirectWrite.TextLayout(Core.Globals.DirectWriteFactory, cellName == "Signal" ? !signal ? "ABS" : "" : eachCellValue.ToString() + (cellName == "Delta/Vol" ? "%" : ""), reuseTextFormat, eachTextWidth, eachTextHeight);
			float widthLayout = reuseTextLayout.Metrics.LayoutWidth;
			reuseVector = new SharpDX.Vector2(x - widthLayout/2 + 2, eachTextYPanel); //x - widthLayout/2 + 2, ChartPanel.Y + ChartPanel.H - 151
			RenderTarget.DrawTextLayout(reuseVector, reuseTextLayout, cellName == "Signal" ? UnFinishAuctColor.ToDxBrush(RenderTarget) : TextColor.ToDxBrush(RenderTarget));
			reuseTextLayout.Dispose();
			reuseTextFormat.Dispose();
			
			//Celdas fijas de nombre de valores
			reuseRect = new SharpDX.RectangleF(eachXFixedCell, eachYFixedCell, eachWidthFixedCell, eachHeightFixedCell);
			RenderTarget.FillRectangle(reuseRect, VolColor.ToDxBrush(RenderTarget));
			
			//Texto fijo de nombre de valores
			reuseTextFormat = new SharpDX.DirectWrite.TextFormat(Core.Globals.DirectWriteFactory, Font.Family.ToString(), SharpDX.DirectWrite.FontWeight.DemiBold, SharpDX.DirectWrite.FontStyle.Normal, 12);
			reuseTextFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading;
			reuseTextLayout = new SharpDX.DirectWrite.TextLayout(Core.Globals.DirectWriteFactory, cellName, reuseTextFormat, eachTextWidth, eachTextHeight);
			float widthFixedLayout = reuseTextLayout.Metrics.LayoutWidth;
			reuseVector = new SharpDX.Vector2(eachWidthFixedText, eachTextYPanel);
			RenderTarget.DrawTextLayout(reuseVector, reuseTextLayout, TextColor.ToDxBrush(RenderTarget));
			reuseTextFormat.Dispose();
			reuseTextLayout.Dispose();
		}
		
		private void VABrackets(float x, float y, float xHor, float yHor, System.Windows.Media.Brush color, bool vertical)
		{
			reuseVector.X		= x;
			reuseVector.Y		= y;
			reuseVector2.X		= vertical ? x : xHor;
			reuseVector2.Y		= vertical ? yHor : y;
			SharpDX.Direct2D1.Brush customDXBrushVALine = color.ToDxBrush(RenderTarget);
			RenderTarget.DrawLine(reuseVector, reuseVector2, customDXBrushVALine, 1);
			customDXBrushVALine.Dispose();
		}
		
		///Visuals
		#region FPType
		protected void InsertWPFControls()
		{
			/////////////////////////////////////////////////////////////////
//			// Define the path to your .ico file
////			string iconFilePath = @"C:\Path\To\YourIcon.ico"; // Replace with your actual file path NinjaTrader.Core.Globals.UserDataDir + "/templates/Indicator/IotecPNG"
			string iconFilePath = NinjaTrader.Core.Globals.UserDataDir + "templates\\indicator\\" + this.Name + "\\iotec-blanco.ico";
//			string pathIconHist = NinjaTrader.Core.Globals.UserDataDir + "templates\\indicator\\" + this.Name + "\\menu_icons\\hist_white.ico";
//			string pathIconBA = NinjaTrader.Core.Globals.UserDataDir + "templates\\indicator\\" + this.Name + "\\menu_icons\\ba_white_new.ico";
			
			// Ensure the file exists
			if (System.IO.File.Exists(iconFilePath))
			{
			    // Load the .ico file using IconBitmapDecoder
			    IconBitmapDecoder ibd = new IconBitmapDecoder(
			        new Uri(iconFilePath, UriKind.Absolute),
			        BitmapCreateOptions.None,
			        BitmapCacheOption.Default);
			
			    // Create an Image control
			    iconImage = new System.Windows.Controls.Image();
			    iconImage.Source = ibd.Frames[0]; // Use the first frame of the icon
			    iconImage.Width = 16;
			    iconImage.Height = 16;
			}
			else
			{
			    // Handle the case where the file does not exist
			    Print("Icon file not found: " + iconFilePath);
			}
			
			
//			// Ensure the file exists
//			if (System.IO.File.Exists(pathIconHist))
//			{
//			    // Load the .ico file using IconBitmapDecoder
//			    IconBitmapDecoder ibd = new IconBitmapDecoder(
//			        new Uri(pathIconHist, UriKind.Absolute),
//			        BitmapCreateOptions.None,
//			        BitmapCacheOption.Default);
			
//			    // Create an Image control
//			    iconMenuTypeFPHist = new System.Windows.Controls.Image();
//			    iconMenuTypeFPHist.Source = ibd.Frames[0]; // Use the first frame of the icon
//			    iconMenuTypeFPHist.Width = 16;
//			    iconMenuTypeFPHist.Height = 16;
//			}
//			else
//			{
//			    // Handle the case where the file does not exist
//			    Print("Icon file not found: " + pathIconHist);
//			}
			
			
//			// Ensure the file exists
//			if (System.IO.File.Exists(pathIconBA))
//			{
//			    // Load the .ico file using IconBitmapDecoder
//			    IconBitmapDecoder ibd = new IconBitmapDecoder(
//			        new Uri(pathIconBA, UriKind.Absolute),
//			        BitmapCreateOptions.None,
//			        BitmapCacheOption.Default);
			
//			    // Create an Image control
//			    iconMenuTypeFPBA = new System.Windows.Controls.Image();
//			    iconMenuTypeFPBA.Source = ibd.Frames[0]; // Use the first frame of the icon
//			    iconMenuTypeFPBA.Width = 16;
//			    iconMenuTypeFPBA.Height = 16;
//			}
//			else
//			{
//			    // Handle the case where the file does not exist
//			    Print("Icon file not found: " + pathIconBA);
//			}
			/////////////////////////////////////////////////////////////////
			
			chartWindow = System.Windows.Window.GetWindow(ChartControl.Parent) as Chart;

			foreach (System.Windows.DependencyObject item in chartWindow.MainMenu)
				if (System.Windows.Automation.AutomationProperties.GetAutomationId(item) == "FootPrint")
					return;

			// this is the actual object that you add to the chart windows Main Menu
			// which will act as a container for all the menu items
			theMenu = new System.Windows.Controls.Menu
			{
				// important to set the alignment, otherwise you will never see the menu populated
				VerticalAlignment			= VerticalAlignment.Top,
				VerticalContentAlignment	= VerticalAlignment.Top,

				// make sure to style as a System Menu	
				Style						= System.Windows.Application.Current.TryFindResource("SystemMenuStyle") as Style
			};

			System.Windows.Automation.AutomationProperties.SetAutomationId(theMenu, "FootPrint");

			// this is the menu item which will appear on the chart's Main Menu
			topMenuItem = new Gui.Tools.NTMenuItem()
			{
				Header				= "iotecFootPrint",
				Foreground			= TextColor,
				Icon				= iconImage,
				Margin				= new System.Windows.Thickness(0),
//				Padding				= new System.Windows.Thickness(1),
				VerticalAlignment	= VerticalAlignment.Center,
				Style				= System.Windows.Application.Current.TryFindResource("MainMenuItem") as Style
			};
			
			theMenu.Items.Add(topMenuItem);	
			
			topMenuTYpeOF = new Gui.Tools.NTMenuItem()
			{
				Header				= IsBidAsk ? "BA" : "∆V",
				Foreground			= TextColor,
//				Icon				= IsBidAsk ? iconMenuTypeFPBA : iconMenuTypeFPHist,
				Margin				= new System.Windows.Thickness(0),
				Padding				= new System.Windows.Thickness(2),
				VerticalAlignment	= VerticalAlignment.Center,
				Style				= System.Windows.Application.Current.TryFindResource("MainMenuItem") as Style
			};

			topMenuTYpeOF.Click += topMenuTYpeOF_Click;
			theMenu.Items.Add(topMenuTYpeOF);	
			
			
			///Type of FootPrint Menu
//			topMenuItemSubItem1 = new Gui.Tools.NTMenuItem()
//			{
//				BorderThickness		= new System.Windows.Thickness(0),
////				Header				= MyFPType == FPType.BidAsk ? "BidAsk" : MyFPType == FPType.Delta ? "Delta" : "Volume",
////				Header				= "Type Foot Print: " + (IsBidAsk ? "BidAsk" : "Delta/Volume"),
//				Header				= "FootPrint Type",
//				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
//				Foreground			= TextColor
//			};

//			topMenuItem.Items.Add(topMenuItemSubItem1);
			
//			topMenuItemSubItem11 = new Gui.Tools.NTMenuItem()
//			{
//				Header				= "Bid|Ask" + (IsBidAsk ? " ✓" : ""),
//				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
//				Foreground			= IsBidAsk ? TextColor : Brushes.Gray
			
//			};
			
//			topMenuItemSubItem11.Click += TopMenuItem1SubItem11_Click;
//			topMenuItemSubItem1.Items.Add(topMenuItemSubItem11);
			
//			topMenuItemSubItem12 = new Gui.Tools.NTMenuItem()
//			{
//				Header				= "Delta|Volume" + (IsBidAsk ? "" : " ✔️"),
//				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
//				Foreground			= IsBidAsk ? Brushes.Gray : TextColor
			
//			};
			
//			topMenuItemSubItem12.Click += TopMenuItem1SubItem12_Click;
//			topMenuItemSubItem1.Items.Add(topMenuItemSubItem12);
			
			///Show Text
			topMenuItemSubItem2 = new Gui.Tools.NTMenuItem()
			{
				Header				= "Text",
				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
				Foreground			= TextColor
			};
			topMenuItem.Items.Add(topMenuItemSubItem2);
			
			topMenuItemSubItem21 = new Gui.Tools.NTMenuItem()
			{
				Header				= "Bid Values" + (ShowTextB ? " ✓" : ""),
				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
				Foreground			= ShowTextB ? TextColor : Brushes.Gray
			
			};
			
			topMenuItemSubItem21.Click += TopMenuItem1SubItem21_Click;
			topMenuItem.Items.Add(topMenuItemSubItem21);
			
			topMenuItemSubItem22 = new Gui.Tools.NTMenuItem()
			{
				Header				= "Ask Values" + (ShowTextA ? " ✓" : ""),
				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
				Foreground			= ShowTextA ? TextColor : Brushes.Gray
			
			};
			
			topMenuItemSubItem22.Click += TopMenuItem1SubItem22_Click;
			topMenuItem.Items.Add(topMenuItemSubItem22);
			
			topMenuItemSubItem23 = new Gui.Tools.NTMenuItem()
			{
				Header				= "Delta Values" + (ShowTextD ? " ✓" : ""),
				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
				Foreground			= ShowTextD ? TextColor : Brushes.Gray
			
			};
			
			topMenuItemSubItem23.Click += TopMenuItem1SubItem23_Click;
			topMenuItem.Items.Add(topMenuItemSubItem23);
			
			topMenuItemSubItem24 = new Gui.Tools.NTMenuItem()
			{
				Header				= "Volume Values" + (ShowTextV ? " ✓" : ""),
				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
				Foreground			= ShowTextV ? TextColor : Brushes.Gray
			};
			
			topMenuItemSubItem24.Click += TopMenuItem1SubItem24_Click;
			topMenuItem.Items.Add(topMenuItemSubItem24);
			
			
			///Bar Internas
			topMenuItemSubItem3 = new Gui.Tools.NTMenuItem()
			{
				Header				= "Bar Iternals",
				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
				Foreground			= TextColor
			};
			topMenuItem.Items.Add(topMenuItemSubItem3);
			
			topMenuItemSubItem31 = new Gui.Tools.NTMenuItem()
			{
				Header				= "Delta Histogram" + (ShowHistDelta ? " ✓" : ""),
				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
				Foreground			= ShowHistDelta ? TextColor : Brushes.Gray
			};
			
			topMenuItemSubItem31.Click += TopMenuItem1SubItem31_Click;
			topMenuItemSubItem3.Items.Add(topMenuItemSubItem31);
			
			topMenuItemSubItem32 = new Gui.Tools.NTMenuItem()
			{
				Header				= "Volume Histogram" + (ShowHistVol ? " ✓" : ""),
				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
				Foreground			= ShowHistVol ? TextColor : Brushes.Gray
			};
			
			topMenuItemSubItem32.Click += TopMenuItem1SubItem32_Click;
			topMenuItemSubItem3.Items.Add(topMenuItemSubItem32);
			
			topMenuItemSubItem33 = new Gui.Tools.NTMenuItem()
			{
				Header				= "Imbalances " + ImbFact.ToString() + ":1" + (ShowImb ? " ✓" : ""),
				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
				Foreground			= ShowImb ? TextColor : Brushes.Gray
			};
	
			topMenuItemSubItem33.Click += TopMenuItem1SubItem33_Click;
			topMenuItemSubItem3.Items.Add(topMenuItemSubItem33);
			
			topMenuItemSubItem34 = new Gui.Tools.NTMenuItem()
			{
				Header				= (ImbType ? "Diagonal " : "Horizontal ") + "Imbalances " + (ImbType ? "⤢" : "⟷"),
				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
				Foreground			= TextColor
			};
	
			topMenuItemSubItem34.Click += TopMenuItem1SubItem34_Click;
			topMenuItemSubItem3.Items.Add(topMenuItemSubItem34);
			
			topMenuItemSubItem35 = new Gui.Tools.NTMenuItem()
			{
				Header				= "Stacked Imbalances of " + StckImbNum.ToString() + (ShowStckImb ? " ✓" : ""),
				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
				Foreground			= ShowStckImb ? TextColor : Brushes.Gray
			
			};
			
			topMenuItemSubItem35.Click += TopMenuItem1SubItem35_Click;
			topMenuItemSubItem3.Items.Add(topMenuItemSubItem35);
			
			topMenuItemSubItem36 = new Gui.Tools.NTMenuItem()
			{
				Header				= "Value Area " + ValueAreaPer.ToString() + "%" + (ShowVA ? " ✓" : ""),
				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
				Foreground			= ShowVA ? TextColor : Brushes.Gray
			};

			topMenuItemSubItem36.Click += TopMenuItem1SubItem36_Click;
			topMenuItemSubItem3.Items.Add(topMenuItemSubItem36);
			
			///By Bar
			topMenuItemSubItem4 = new Gui.Tools.NTMenuItem()
			{
				Header				= "By Bar",
				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
				Foreground			= TextColor
			};
			topMenuItem.Items.Add(topMenuItemSubItem4);
			
			topMenuItemSubItem41 = new Gui.Tools.NTMenuItem()
			{
				Header				= "Ratios" + (ShowRatios ? " ✓" : ""),
				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
				Foreground			= ShowRatios ? TextColor : Brushes.Gray
			};

			topMenuItemSubItem41.Click += TopMenuItem1SubItem41_Click;
			topMenuItemSubItem4.Items.Add(topMenuItemSubItem41);
			
			topMenuItemSubItem42 = new Gui.Tools.NTMenuItem()
			{
				Header				= "Absorptions" + (ShowABS ? " ✓" : ""),
				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
				Foreground			= ShowABS ? TextColor : Brushes.Gray
			};

			topMenuItemSubItem42.Click += TopMenuItem1SubItem42_Click;
			topMenuItemSubItem4.Items.Add(topMenuItemSubItem42);
			
			topMenuItemSubItem43 = new Gui.Tools.NTMenuItem()
			{
				Header				= "HOD/LOD" + (ShowHLOD ? " ✓" : ""),
				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
				Foreground			= ShowHLOD ? TextColor : Brushes.Gray
			};

			topMenuItemSubItem43.Click += TopMenuItem1SubItem43_Click;
			topMenuItemSubItem4.Items.Add(topMenuItemSubItem43);
			
//			topMenuItemSubItem44 = new Gui.Tools.NTMenuItem()
//			{
//				Header				= "Big Trades" + (ShowBT ? " ✓" : ""),
//				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
//				Foreground			= ShowBT ? TextColor : Brushes.Gray
//			};

//			topMenuItemSubItem44.Click += TopMenuItem1SubItem44_Click;
//			topMenuItemSubItem4.Items.Add(topMenuItemSubItem44);
			
			topMenuItemSubItem44 = new Gui.Tools.NTMenuItem()
			{
				Header				= "Big Trades",
				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
				Foreground			= TextColor
			};

			topMenuItemSubItem4.Items.Add(topMenuItemSubItem44);

			topMenuItemSubItem441 = new Gui.Tools.NTMenuItem()
			{
				Header				= "BT Circles " + "(" + BTVol + ")" + (ShowBTCircles ? " ✓" : ""),
				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
				Foreground			= ShowBTCircles ? TextColor : Brushes.Gray
			};

			topMenuItemSubItem441.Click += TopMenuItem1SubItem441_Click;
			topMenuItemSubItem44.Items.Add(topMenuItemSubItem441);
			
			topMenuItemSubItem442 = new Gui.Tools.NTMenuItem()
			{
				Header				= "BT Lines S/R " + "(" + BTVolLVLSR + "-" + BTVolHVLSR + ")"  + (ShowBTLSR ? " ✓" : ""),
				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
				Foreground			= ShowBTLSR ? TextColor : Brushes.Gray
			};

			topMenuItemSubItem442.Click += TopMenuItem1SubItem442_Click;
			topMenuItemSubItem44.Items.Add(topMenuItemSubItem442);
			
			topMenuItemSubItem443 = new Gui.Tools.NTMenuItem()
			{
				Header				= "BT text for Lines S/R" + (BTLSRText ? " ✓" : ""),
				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
				Foreground			= BTLSRText ? TextColor : Brushes.Gray
			};

			topMenuItemSubItem443.Click += TopMenuItem1SubItem443_Click;
			topMenuItemSubItem44.Items.Add(topMenuItemSubItem443);
			
			//BTLSRText
			
			topMenuItemSubItem5 = new Gui.Tools.NTMenuItem()
			{
				Header				= "Session Volume Profile" + (ShowVP ? " ✓" : ""),
				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
				Foreground			= ShowVP ? TextColor : Brushes.Gray
			};

			topMenuItemSubItem5.Click += TopMenuItem1SubItem5_Click;
			topMenuItem.Items.Add(topMenuItemSubItem5);
			
			topMenuItemSubItem6 = new Gui.Tools.NTMenuItem()
			{
				Header				= "Summary Table" + (ShowSummTab ? " ✓" : ""),
				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
				Foreground			= ShowSummTab ? TextColor : Brushes.Gray
			};

			topMenuItemSubItem6.Click += TopMenuItem1SubItem6_Click;
			topMenuItem.Items.Add(topMenuItemSubItem6);
			
//// Create the submenu item
//topMenuItemSubItem1 = new Gui.Tools.NTMenuItem()
//{
//    BorderThickness     = new System.Windows.Thickness(0),
//    Header              = IsBidAsk ? "Volume" : "BidAsk",
//    Style               = System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
//    Foreground          = TextColor
//};

//// Add the submenu item to the parent menu item
//topMenuItem.Items.Add(topMenuItemSubItem1);

//			topMenuItemSubItem2 = new Gui.Tools.NTMenuItem()
//			{
//				Header				= ShowText ? "Hide Text" : "Show Text",
//				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
//				Foreground			= ShowText ? TextColor : Brushes.Gray

//			};

//			topMenuItemSubItem2.Click += TopMenuItem1SubItem2_Click;
//			topMenuItem.Items.Add(topMenuItemSubItem2);	
	
//			topMenuItemSubItem3 = new Gui.Tools.NTMenuItem()
//			{
//				Header				= ShowImb ? "Hide Imbalances" : "Show Imbalances",
//				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
//				Foreground			= ShowImb ? TextColor : Brushes.Gray
//			};

//			topMenuItemSubItem3.Click += TopMenuItem1SubItem3_Click;
//			topMenuItem.Items.Add(topMenuItemSubItem3);
			
//			topMenuItemSubItem4 = new Gui.Tools.NTMenuItem()
//			{
//				Header				= ShowStckImb ? "Hide Stacked Imbalances" : "Show Stacked Imbalances",
//				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
//				Foreground			= ShowStckImb ? TextColor : Brushes.Gray
//			};

//			topMenuItemSubItem4.Click += TopMenuItem1SubItem4_Click;
//			topMenuItem.Items.Add(topMenuItemSubItem4);
			
//			topMenuItemSubItem5 = new Gui.Tools.NTMenuItem()
//			{
//				Header				= ShowRatios ? "Hide Ratios" : "Show ratios",
//				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
//				Foreground			= ShowRatios ? TextColor : Brushes.Gray
//			};

//			topMenuItemSubItem5.Click += TopMenuItem1SubItem5_Click;
//			topMenuItem.Items.Add(topMenuItemSubItem5);
			
//			topMenuItemSubItem6 = new Gui.Tools.NTMenuItem()
//			{
//				Header				= ShowHistVol ? "Hide Volume Histogram" : "Show Volume Histogram",
//				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
//				Foreground			= ShowHistVol ? TextColor : Brushes.Gray
//			};
 
//			topMenuItemSubItem6.Click += TopMenuItem1SubItem6_Click;
//			topMenuItem.Items.Add(topMenuItemSubItem6);
			
//			topMenuItemSubItem7 = new Gui.Tools.NTMenuItem()
//			{
//				Header				= ShowHistDelta ? "Hide Delta Histogram" : "Show Delta Histogram",
//				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
//				Foreground			= ShowHistDelta ? TextColor : Brushes.Gray
//			};

//			topMenuItemSubItem7.Click += TopMenuItem1SubItem7_Click;
//			topMenuItem.Items.Add(topMenuItemSubItem7);
			
//			topMenuItemSubItem8 = new Gui.Tools.NTMenuItem()
//			{
//				Header				= ShowParInfo ? "Hide Parameters info" : "Show Parameters info",
//				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
//				Foreground			= ShowParInfo ? TextColor : Brushes.Gray
//			};

//			topMenuItemSubItem8.Click += TopMenuItem1SubItem8_Click;
//			topMenuItem.Items.Add(topMenuItemSubItem8);

//			topMenuItemSubItem9 = new Gui.Tools.NTMenuItem()
//			{
//				Header				= ShowSummTab ? "Hide Summary table" : "Show Summary table",
//				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
//				Foreground			= ShowSummTab ? TextColor : Brushes.Gray
//			};

//			topMenuItemSubItem9.Click += TopMenuItem1SubItem9_Click;
//			topMenuItem.Items.Add(topMenuItemSubItem9);
			
//			topMenuItemSubItem10 = new Gui.Tools.NTMenuItem()
//			{
//				Header				= ShowBT ? "Hide Big Trades" : "Show Big Trades",
//				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
//				Foreground			= ShowBT ? TextColor : Brushes.Gray
//			};

//			topMenuItemSubItem10.Click += TopMenuItem1SubItem10_Click;
//			topMenuItem.Items.Add(topMenuItemSubItem10);
			
//			topMenuItemSubItem11 = new Gui.Tools.NTMenuItem()
//			{
//				Header				= ShowHLOD ? "Hide HOD/LOD" : "Show HOD/LOD",
//				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
//				Foreground			= ShowHLOD ? TextColor : Brushes.Gray
//			};

//			topMenuItemSubItem11.Click += TopMenuItem1SubItem11_Click;
//			topMenuItem.Items.Add(topMenuItemSubItem11);
			
//			topMenuItemSubItem12 = new Gui.Tools.NTMenuItem()
//			{
//				Header				= MinWidth ? "Bar separation active" : "Bar separation inactive",
//				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
//				Foreground			= MinWidth ? TextColor : Brushes.Gray
//			};

//			topMenuItemSubItem12.Click += TopMenuItem1SubItem12_Click;
//			topMenuItem.Items.Add(topMenuItemSubItem12);
			
//			topMenuItemSubItem13 = new Gui.Tools.NTMenuItem()
//			{
//				Header				= ShowVP ? "Hide Session VP" : "Show Session VP",
//				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
//				Foreground			= ShowVP ? TextColor : Brushes.Gray
//			};

//			topMenuItemSubItem13.Click += TopMenuItem1SubItem13_Click;
//			topMenuItem.Items.Add(topMenuItemSubItem13);
			
//			topMenuItemSubItem14 = new Gui.Tools.NTMenuItem()
//			{
//				Header				= ShowABS ? "Hide Absorptions" : "Show Absorptions",
//				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
//				Foreground			= ShowABS ? TextColor : Brushes.Gray
//			};

//			topMenuItemSubItem14.Click += TopMenuItem1SubItem14_Click;
//			topMenuItem.Items.Add(topMenuItemSubItem14);
			
//			topMenuItemSubItem15 = new Gui.Tools.NTMenuItem()
//			{
//				Header				= ShowVA ? "Hide Value Area" : "Show Value Area",
//				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style,
//				Foreground			= ShowVA ? TextColor : Brushes.Gray
//			};

//			topMenuItemSubItem15.Click += TopMenuItem1SubItem15_Click;
//			topMenuItem.Items.Add(topMenuItemSubItem15);
			
			// add the menu which contains all menu items to the chart
			chartWindow.MainMenu.Add(theMenu);

			foreach (System.Windows.Controls.TabItem tab in chartWindow.MainTabControl.Items)
				if ((tab.Content as ChartTab).ChartControl == ChartControl && tab == chartWindow.MainTabControl.SelectedItem)
					topMenuItem.Visibility = Visibility.Visible;

			chartWindow.MainTabControl.SelectionChanged += MySelectionChangedHandler;
		}

		private void MySelectionChangedHandler(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count <= 0)
				return;

			tabItem = e.AddedItems[0] as System.Windows.Controls.TabItem;
			if (tabItem == null)
				return;

			chartTab = tabItem.Content as NinjaTrader.Gui.Chart.ChartTab; 
			if (chartTab != null)
				if (topMenuItem != null)
					topMenuItem.Visibility = chartTab.ChartControl == ChartControl ? Visibility.Visible : Visibility.Collapsed;
		}
		
		protected void RemoveWPFControls()
		{
			if (topMenuTYpeOF != null)
				topMenuTYpeOF.Click -= topMenuTYpeOF_Click;
			//topMenuTYpeOF_Click
			
			if (topMenuItemSubItem11 != null)
				topMenuItemSubItem11.Click -= TopMenuItem1SubItem11_Click;
			
			if (topMenuItemSubItem12 != null)
				topMenuItemSubItem12.Click -= TopMenuItem1SubItem12_Click;
			
//			if (topMenuItemSubItem21 != null)
//				topMenuItemSubItem21.Click -= TopMenuItem1SubItem21_Click;
			
			if (topMenuItemSubItem22 != null)
				topMenuItemSubItem22.Click -= TopMenuItem1SubItem22_Click;
			
			if (topMenuItemSubItem23 != null)
				topMenuItemSubItem23.Click -= TopMenuItem1SubItem23_Click;
			
			if (topMenuItemSubItem24 != null)
				topMenuItemSubItem24.Click -= TopMenuItem1SubItem24_Click;
			
			if (topMenuItemSubItem31 != null)
				topMenuItemSubItem31.Click -= TopMenuItem1SubItem31_Click;
			
			if (topMenuItemSubItem32 != null)
				topMenuItemSubItem32.Click -= TopMenuItem1SubItem32_Click;
			
			if (topMenuItemSubItem33 != null)
				topMenuItemSubItem33.Click -= TopMenuItem1SubItem33_Click;
			
			if (topMenuItemSubItem34 != null)
				topMenuItemSubItem34.Click -= TopMenuItem1SubItem34_Click;
			
			if (topMenuItemSubItem35 != null)
				topMenuItemSubItem35.Click -= TopMenuItem1SubItem35_Click;
			
			if (topMenuItemSubItem36 != null)
				topMenuItemSubItem36.Click -= TopMenuItem1SubItem36_Click;
			
//			if (topMenuItemSubItem37 != null)
//				topMenuItemSubItem37.Click -= TopMenuItem1SubItem37_Click;
			
//			if (topMenuItemSubItem4 != null)
//				topMenuItemSubItem4.Click -= TopMenuItem1SubItem4_Click;
			
			if (topMenuItemSubItem41 != null)
				topMenuItemSubItem41.Click -= TopMenuItem1SubItem41_Click;
			
			if (topMenuItemSubItem42 != null)
				topMenuItemSubItem42.Click -= TopMenuItem1SubItem42_Click;
			
			if (topMenuItemSubItem43 != null)
				topMenuItemSubItem43.Click -= TopMenuItem1SubItem43_Click;
			
//			if (topMenuItemSubItem44 != null)
//				topMenuItemSubItem44.Click -= TopMenuItem1SubItem44_Click;
			
			if (topMenuItemSubItem441 != null)
				topMenuItemSubItem441.Click -= TopMenuItem1SubItem441_Click;
			
			if (topMenuItemSubItem442 != null)
				topMenuItemSubItem442.Click -= TopMenuItem1SubItem442_Click;
			
			if (topMenuItemSubItem5 != null)
				topMenuItemSubItem5.Click -= TopMenuItem1SubItem5_Click;
			
			if (topMenuItemSubItem6 != null)
				topMenuItemSubItem6.Click -= TopMenuItem1SubItem6_Click;
			
//			if (topMenuItemSubItem7 != null)
//				topMenuItemSubItem7.Click -= TopMenuItem1SubItem7_Click;
			
//			if (topMenuItemSubItem8 != null)
//				topMenuItemSubItem8.Click -= TopMenuItem1SubItem8_Click;
						
//			if (topMenuItemSubItem9 != null)
//				topMenuItemSubItem9.Click -= TopMenuItem1SubItem9_Click;
			
//			if (topMenuItemSubItem10 != null)
//				topMenuItemSubItem10.Click -= TopMenuItem1SubItem10_Click;
			
//			if (topMenuItemSubItem11 != null)
//				topMenuItemSubItem11.Click -= TopMenuItem1SubItem11_Click;
			
//			if (topMenuItemSubItem12 != null)
//				topMenuItemSubItem12.Click -= TopMenuItem1SubItem12_Click;
			
//			if (topMenuItemSubItem13 != null)
//				topMenuItemSubItem13.Click -= TopMenuItem1SubItem13_Click;
			
//			if (topMenuItemSubItem14 != null)
//				topMenuItemSubItem14.Click -= TopMenuItem1SubItem14_Click;
			
//			if (topMenuItemSubItem15 != null)
//				topMenuItemSubItem15.Click -= TopMenuItem1SubItem15_Click;

			if (theMenu != null)
				chartWindow.MainMenu.Remove(theMenu);

			chartWindow.MainTabControl.SelectionChanged -= MySelectionChangedHandler;
		}

		//topMenuTYpeOF_Click
		protected void topMenuTYpeOF_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			IsBidAsk = !IsBidAsk;
			topMenuTYpeOF.Foreground 	= TextColor;
//			topMenuItemSubItem12.Foreground 	= Brushes.Gray;
			topMenuTYpeOF.Header			= IsBidAsk ? "BA" : "∆V"; //🅱️🅰️ B|A 📊
//			topMenuItemSubItem12.Header 		= "Delta|Volume";
//			topMenuTYpeOF.Icon = IsBidAsk ? iconMenuTypeFPBA : iconMenuTypeFPHist;
			
			ChartControl.InvalidateVisual();
		}
		
		//Action for Type of FootPrint
		protected void TopMenuItem1SubItem11_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			IsBidAsk = true;
			topMenuItemSubItem11.Foreground 	= TextColor;
			topMenuItemSubItem12.Foreground 	= Brushes.Gray;
			topMenuItemSubItem11.Header			= "Bid|Ask" + (IsBidAsk ? " ✓" : "");
			topMenuItemSubItem12.Header 		= "Delta|Volume";
			
			ChartControl.InvalidateVisual();
		}
		
		protected void TopMenuItem1SubItem12_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			IsBidAsk = false;
			topMenuItemSubItem12.Foreground 	= TextColor;
			topMenuItemSubItem11.Foreground 	= Brushes.Gray;
			topMenuItemSubItem12.Header			= "Delta|Volume" + (IsBidAsk ? "" : " ✓");
			topMenuItemSubItem11.Header			= "Bid|Ask";
			ChartControl.InvalidateVisual();
		}
		
		//Action for Text
		protected void TopMenuItem1SubItem21_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			ShowTextB = !ShowTextB;
			topMenuItemSubItem21.Foreground 	= ShowTextB ? TextColor : Brushes.Gray;
			topMenuItemSubItem21.Header			= "Bid Values" + (ShowTextB ? " ✓" : "");
			ChartControl.InvalidateVisual();
		}
		
		protected void TopMenuItem1SubItem22_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			ShowTextA = !ShowTextA;
			topMenuItemSubItem22.Foreground 	= ShowTextA ? TextColor : Brushes.Gray;
			topMenuItemSubItem22.Header			= "Ask Values" + (ShowTextA ? " ✓" : "");
			ChartControl.InvalidateVisual();
		}
		
		protected void TopMenuItem1SubItem23_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			ShowTextD = !ShowTextD;
			topMenuItemSubItem23.Foreground 	= ShowTextD ? TextColor : Brushes.Gray;
			topMenuItemSubItem23.Header			= "Delta Values" + (ShowTextD ? " ✓" : "");
			ChartControl.InvalidateVisual();
		}
		
		protected void TopMenuItem1SubItem24_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			ShowTextV = !ShowTextV;
			topMenuItemSubItem24.Foreground 	= ShowTextV ? TextColor : Brushes.Gray;
			topMenuItemSubItem24.Header			= "Volume Values" + (ShowTextV ? " ✓" : "");
			ChartControl.InvalidateVisual();
		}
		
		//Action Internals
		protected void TopMenuItem1SubItem31_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			ShowHistDelta = !ShowHistDelta;
			topMenuItemSubItem31.Foreground 	= ShowHistDelta ? TextColor : Brushes.Gray;
			topMenuItemSubItem31.Header			= "Delta Histogram" + (ShowHistDelta ? " ✓" : "");
			ChartControl.InvalidateVisual();
		}
		
		protected void TopMenuItem1SubItem32_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			ShowHistVol = !ShowHistVol;
			topMenuItemSubItem32.Foreground 	= ShowHistVol ? TextColor : Brushes.Gray;
			topMenuItemSubItem32.Header			= "Volume Histogram" + (ShowHistVol ? " ✓" : "");
			ChartControl.InvalidateVisual();
		}
		
		protected void TopMenuItem1SubItem33_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			ShowImb = !ShowImb;
			topMenuItemSubItem33.Foreground 	= ShowImb ? TextColor : Brushes.Gray;
			topMenuItemSubItem33.Header			= "Imbalances " + ImbFact.ToString() + ":1" + (ShowImb ? " ✓" : "");
			ChartControl.InvalidateVisual();
		}
		
		protected void TopMenuItem1SubItem34_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			ImbType = !ImbType;
			topMenuItemSubItem34.Foreground 	= TextColor;
			topMenuItemSubItem34.Header			= (ImbType ? "Diagonal " : "Horizontal ") + "Imbalances " + (ImbType ? "⤢" : "⟷");
			ChartControl.InvalidateVisual();
		}
				
		protected void TopMenuItem1SubItem35_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			ShowStckImb = !ShowStckImb;
			topMenuItemSubItem35.Foreground 	= ShowStckImb ? TextColor : Brushes.Gray;
			topMenuItemSubItem35.Header			= "Stacked Imbalances of " + StckImbNum.ToString() + (ShowStckImb ? " ✓" : "");
			ChartControl.InvalidateVisual();
		}
				
		protected void TopMenuItem1SubItem36_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			ShowVA = !ShowVA;
			topMenuItemSubItem36.Foreground 	= ShowVA ? TextColor : Brushes.Gray;
			topMenuItemSubItem36.Header			= "Value Area " + ValueAreaPer.ToString() + "%" + (ShowVA ? " ✓" : "") ;
			ChartControl.InvalidateVisual();
		}
		
		
		///By Bar
		protected void TopMenuItem1SubItem41_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			ShowRatios = !ShowRatios;
			topMenuItemSubItem41.Foreground 	= ShowRatios ? TextColor : Brushes.Gray;
//			topMenuItemSubItem41.Foreground 	= TextColor;
			topMenuItemSubItem41.Header		= "Ratios" + (ShowRatios ? " ✓" : "");
			ChartControl.InvalidateVisual();
		}
		
		protected void TopMenuItem1SubItem42_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			ShowABS = !ShowABS;
			topMenuItemSubItem42.Foreground 	= ShowABS ? TextColor : Brushes.Gray;
//			topMenuItemSubItem42.Foreground 	= TextColor;
			topMenuItemSubItem42.Header		= "Absorptions" + (ShowABS ? " ✓" : "");
			ChartControl.InvalidateVisual();
		}
		
		protected void TopMenuItem1SubItem43_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			ShowHLOD = !ShowHLOD;
			topMenuItemSubItem43.Foreground 	= ShowHLOD ? TextColor : Brushes.Gray;
//			topMenuItemSubItem43.Foreground 	= TextColor;
			topMenuItemSubItem43.Header		= "HOD/LOD" + (ShowHLOD ? " ✓" : "");
			ChartControl.InvalidateVisual();
		}
		
//		protected void TopMenuItem1SubItem44_Click(object sender, System.Windows.RoutedEventArgs e)
//		{
//			ShowBT = !ShowBT;
//			topMenuItemSubItem44.Foreground 	= ShowBT ? TextColor : Brushes.Gray;
////			topMenuItemSubItem43.Foreground 	= TextColor;
//			topMenuItemSubItem44.Header		= "Big Trades" + (ShowBT ? " ✓" : "");
//			ChartControl.InvalidateVisual();
//		}
		
		protected void TopMenuItem1SubItem441_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			ShowBTCircles = !ShowBTCircles;
			topMenuItemSubItem441.Foreground 	= ShowBTCircles ? TextColor : Brushes.Gray;
//			topMenuItemSubItem43.Foreground 	= TextColor;
			topMenuItemSubItem441.Header		= "BT Circles " + "(" + BTVol + ")" + (ShowBTCircles ? " ✓" : "");
			ChartControl.InvalidateVisual();
		}
		
		protected void TopMenuItem1SubItem442_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			ShowBTLSR = !ShowBTLSR;
			topMenuItemSubItem442.Foreground 	= ShowBTLSR ? TextColor : Brushes.Gray;
//			topMenuItemSubItem43.Foreground 	= TextColor;
			topMenuItemSubItem442.Header		= "BT Lines S/R " + "(" + BTVolLVLSR + "-" + BTVolHVLSR + ")" + (ShowBTLSR ? " ✓" : "");
			ChartControl.InvalidateVisual();
		}
		
		protected void TopMenuItem1SubItem443_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			BTLSRText = !BTLSRText;
			topMenuItemSubItem443.Foreground 	= BTLSRText ? TextColor : Brushes.Gray;
//			topMenuItemSubItem43.Foreground 	= TextColor;
			topMenuItemSubItem443.Header		= "BT text for Lines S/R" + (BTLSRText ? " ✓" : "");
			ChartControl.InvalidateVisual();
		}
		
		protected void TopMenuItem1SubItem5_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			ShowVP = !ShowVP;
			topMenuItemSubItem5.Foreground 	= ShowVP ? TextColor : Brushes.Gray;
//			topMenuItemSubItem43.Foreground 	= TextColor;
			topMenuItemSubItem5.Header		= "Session Volume Profile" + (ShowVP ? " ✓" : "");
			ChartControl.InvalidateVisual();
		}
		
		protected void TopMenuItem1SubItem6_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			ShowSummTab = !ShowSummTab;
			topMenuItemSubItem6.Foreground 	= ShowSummTab ? TextColor : Brushes.Gray;
			topMenuItemSubItem6.Header		= "Summary table" + (ShowSummTab ? " ✓" : "");
			ChartControl.InvalidateVisual();
		}
		
//		protected void TopMenuItem1SubItem1_Click(object sender, System.Windows.RoutedEventArgs e)
//		{
//			IsBidAsk = !IsBidAsk;
////			topMenuItemSubItem1.Foreground 	= IsBidAsk ? TextColor : Brushes.Gray;
//			topMenuItemSubItem1.Foreground 	= TextColor;
//			topMenuItemSubItem1.Header		= "Type: " + (IsBidAsk ? "BidAsk" : "Delta/Volume");
//			ChartControl.InvalidateVisual();
//		}
		
//		protected void TopMenuItem1SubItem1_Click(object sender, System.Windows.RoutedEventArgs e)
//		{
//			IsBidAsk = !IsBidAsk;
////			topMenuItemSubItem1.Foreground 	= IsBidAsk ? TextColor : Brushes.Gray;
//			topMenuItemSubItem1.Foreground 	= TextColor;
//			topMenuItemSubItem1.Header		= "Type: " + (IsBidAsk ? "BidAsk" : "Delta/Volume");
//			ChartControl.InvalidateVisual();
//		}

//		protected void TopMenuItem1SubItem2_Click(object sender, System.Windows.RoutedEventArgs e)
//		{
//			ShowText = !ShowText;
//			topMenuItemSubItem2.Foreground 	= ShowText ? TextColor : Brushes.Gray;
//			topMenuItemSubItem2.Header		= ShowText ? "Hide Text" : "Show Text";
//			ChartControl.InvalidateVisual();
//		}
		
//		protected void TopMenuItem1SubItem3_Click(object sender, System.Windows.RoutedEventArgs e)
//		{
//			ShowImb = !ShowImb;
//			topMenuItemSubItem3.Foreground 	= ShowImb ? TextColor : Brushes.Gray;
//			topMenuItemSubItem3.Header		= ShowImb ? "Hide Imbalances" : "Show Imbalances";
//			ChartControl.InvalidateVisual();
//		}
		
//		protected void TopMenuItem1SubItem4_Click(object sender, System.Windows.RoutedEventArgs e)
//		{
//			ShowStckImb = !ShowStckImb;
//			topMenuItemSubItem4.Foreground 	= ShowStckImb ? TextColor : Brushes.Gray;
//			topMenuItemSubItem4.Header		= ShowStckImb ? "Hide Stacked Imbalances" : "Show Stacked Imbalances";
//			ChartControl.InvalidateVisual();
//		}
		
//		protected void TopMenuItem1SubItem5_Click(object sender, System.Windows.RoutedEventArgs e)
//		{
//			ShowRatios = !ShowRatios;
//			topMenuItemSubItem5.Foreground 	= ShowRatios ? TextColor : Brushes.Gray;
//			topMenuItemSubItem5.Header		= ShowRatios ? "Hide Ratios" : "Show Ratios";
//			ChartControl.InvalidateVisual();
//		}
		
//		protected void TopMenuItem1SubItem6_Click(object sender, System.Windows.RoutedEventArgs e)
//		{
//			ShowHistVol = !ShowHistVol;
//			topMenuItemSubItem6.Foreground 	= ShowHistVol ? TextColor : Brushes.Gray;
//			topMenuItemSubItem6.Header		= ShowHistVol ? "Hide Volume Histogram" : "Show Volume Histogram";
//			ChartControl.InvalidateVisual();
//		}
		
//		protected void TopMenuItem1SubItem7_Click(object sender, System.Windows.RoutedEventArgs e)
//		{
//			ShowHistDelta = !ShowHistDelta;
//			topMenuItemSubItem7.Foreground 	= ShowHistDelta ? TextColor : Brushes.Gray;
//			topMenuItemSubItem7.Header		= ShowHistDelta ? "Hide Delta Histogram" : "Show Delta Histogram";
//			ChartControl.InvalidateVisual();
//		}
		
//		protected void TopMenuItem1SubItem8_Click(object sender, System.Windows.RoutedEventArgs e)
//		{
//			ShowParInfo = !ShowParInfo;
//			topMenuItemSubItem8.Foreground 	= ShowParInfo ? TextColor : Brushes.Gray;
//			topMenuItemSubItem8.Header		= ShowParInfo ? "Hide Parameters info" : "Show Parameters info";
//			ChartControl.InvalidateVisual();
//		}
		
//		protected void TopMenuItem1SubItem9_Click(object sender, System.Windows.RoutedEventArgs e)
//		{
//			ShowSummTab = !ShowSummTab;
//			topMenuItemSubItem9.Foreground 	= ShowSummTab ? TextColor : Brushes.Gray;
//			topMenuItemSubItem9.Header		= ShowSummTab ? "Hide Summary table" : "Show Summary table";
//			ChartControl.InvalidateVisual();
//		}
		
//		protected void TopMenuItem1SubItem10_Click(object sender, System.Windows.RoutedEventArgs e)
//		{
//			ShowBT = !ShowBT;
//			topMenuItemSubItem10.Foreground 	= ShowBT ? TextColor : Brushes.Gray;
//			topMenuItemSubItem10.Header			= ShowBT ? "Hide Big Trades" : "Show Big Trades";
//			ChartControl.InvalidateVisual();
//		}
		
//		protected void TopMenuItem1SubItem11_Click(object sender, System.Windows.RoutedEventArgs e)
//		{
//			ShowHLOD = !ShowHLOD;
//			topMenuItemSubItem11.Foreground 	= ShowHLOD ? TextColor : Brushes.Gray;
//			topMenuItemSubItem11.Header			= ShowHLOD ? "Hide HOD/LOD" : "Show HOD/LOD";
//			ChartControl.InvalidateVisual();
//		}
		
//		protected void TopMenuItem1SubItem12_Click(object sender, System.Windows.RoutedEventArgs e)
//		{
//			MinWidth = !MinWidth;
//			topMenuItemSubItem12.Foreground 	= MinWidth ? TextColor : Brushes.Gray;
//			topMenuItemSubItem12.Header			= MinWidth ? "Bar separation active" : "Bar separation inactive";
//			ChartControl.InvalidateVisual();
//		}
		
//		protected void TopMenuItem1SubItem13_Click(object sender, System.Windows.RoutedEventArgs e)
//		{
//			ShowVP = !ShowVP;
//			topMenuItemSubItem13.Foreground 	= ShowVP ? TextColor : Brushes.Gray;
//			topMenuItemSubItem13.Header			= ShowVP ? "Hide Session VP" : "Show Session VP";
//			ChartControl.InvalidateVisual();
//		}
		
//		protected void TopMenuItem1SubItem14_Click(object sender, System.Windows.RoutedEventArgs e)
//		{
//			ShowABS = !ShowABS;
//			topMenuItemSubItem14.Foreground 	= ShowABS ? TextColor : Brushes.Gray;
//			topMenuItemSubItem14.Header			= ShowABS ? "Hide Absorptions" : "Show Absorptions";
//			ChartControl.InvalidateVisual();
//		}
		
//		protected void TopMenuItem1SubItem15_Click(object sender, System.Windows.RoutedEventArgs e)
//		{
//			ShowVA = !ShowVA;
//			topMenuItemSubItem15.Foreground 	= ShowVA ? TextColor : Brushes.Gray;
//			topMenuItemSubItem15.Header			= ShowVA ? "Hide Value Area" : "Show Value Area";
//			ChartControl.InvalidateVisual();
//		}
		#endregion
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private lab.iotecOFPlus4[] cacheiotecOFPlus4;
		public lab.iotecOFPlus4 iotecOFPlus4(System.Windows.Media.Brush barUpColor, System.Windows.Media.Brush barDownColor, System.Windows.Media.Brush dojiColor, System.Windows.Media.Brush wickColor, System.Windows.Media.Brush pocColor, System.Windows.Media.Brush deltaPosColor, System.Windows.Media.Brush deltaNegColor, System.Windows.Media.Brush textColor, System.Windows.Media.Brush askImbColor, System.Windows.Media.Brush bidImbColor, System.Windows.Media.Brush stckAskImbColor, System.Windows.Media.Brush stckBidImbColor, System.Windows.Media.Brush unFinishAuctColor, System.Windows.Media.Brush volColor, System.Windows.Media.Brush volBidColor, System.Windows.Media.Brush volAskColor, System.Windows.Media.Brush vAColor, System.Windows.Media.Brush vPColor, System.Windows.Media.Brush absorptionBidColor, System.Windows.Media.Brush absorptionAskColor)
		{
			return iotecOFPlus4(Input, barUpColor, barDownColor, dojiColor, wickColor, pocColor, deltaPosColor, deltaNegColor, textColor, askImbColor, bidImbColor, stckAskImbColor, stckBidImbColor, unFinishAuctColor, volColor, volBidColor, volAskColor, vAColor, vPColor, absorptionBidColor, absorptionAskColor);
		}

		public lab.iotecOFPlus4 iotecOFPlus4(ISeries<double> input, System.Windows.Media.Brush barUpColor, System.Windows.Media.Brush barDownColor, System.Windows.Media.Brush dojiColor, System.Windows.Media.Brush wickColor, System.Windows.Media.Brush pocColor, System.Windows.Media.Brush deltaPosColor, System.Windows.Media.Brush deltaNegColor, System.Windows.Media.Brush textColor, System.Windows.Media.Brush askImbColor, System.Windows.Media.Brush bidImbColor, System.Windows.Media.Brush stckAskImbColor, System.Windows.Media.Brush stckBidImbColor, System.Windows.Media.Brush unFinishAuctColor, System.Windows.Media.Brush volColor, System.Windows.Media.Brush volBidColor, System.Windows.Media.Brush volAskColor, System.Windows.Media.Brush vAColor, System.Windows.Media.Brush vPColor, System.Windows.Media.Brush absorptionBidColor, System.Windows.Media.Brush absorptionAskColor)
		{
			if (cacheiotecOFPlus4 != null)
				for (int idx = 0; idx < cacheiotecOFPlus4.Length; idx++)
					if (cacheiotecOFPlus4[idx] != null && cacheiotecOFPlus4[idx].BarUpColor == barUpColor && cacheiotecOFPlus4[idx].BarDownColor == barDownColor && cacheiotecOFPlus4[idx].DojiColor == dojiColor && cacheiotecOFPlus4[idx].WickColor == wickColor && cacheiotecOFPlus4[idx].PocColor == pocColor && cacheiotecOFPlus4[idx].DeltaPosColor == deltaPosColor && cacheiotecOFPlus4[idx].DeltaNegColor == deltaNegColor && cacheiotecOFPlus4[idx].TextColor == textColor && cacheiotecOFPlus4[idx].AskImbColor == askImbColor && cacheiotecOFPlus4[idx].BidImbColor == bidImbColor && cacheiotecOFPlus4[idx].StckAskImbColor == stckAskImbColor && cacheiotecOFPlus4[idx].StckBidImbColor == stckBidImbColor && cacheiotecOFPlus4[idx].UnFinishAuctColor == unFinishAuctColor && cacheiotecOFPlus4[idx].VolColor == volColor && cacheiotecOFPlus4[idx].VolBidColor == volBidColor && cacheiotecOFPlus4[idx].VolAskColor == volAskColor && cacheiotecOFPlus4[idx].VAColor == vAColor && cacheiotecOFPlus4[idx].VPColor == vPColor && cacheiotecOFPlus4[idx].AbsorptionBidColor == absorptionBidColor && cacheiotecOFPlus4[idx].AbsorptionAskColor == absorptionAskColor && cacheiotecOFPlus4[idx].EqualsInput(input))
						return cacheiotecOFPlus4[idx];
			return CacheIndicator<lab.iotecOFPlus4>(new lab.iotecOFPlus4(){ BarUpColor = barUpColor, BarDownColor = barDownColor, DojiColor = dojiColor, WickColor = wickColor, PocColor = pocColor, DeltaPosColor = deltaPosColor, DeltaNegColor = deltaNegColor, TextColor = textColor, AskImbColor = askImbColor, BidImbColor = bidImbColor, StckAskImbColor = stckAskImbColor, StckBidImbColor = stckBidImbColor, UnFinishAuctColor = unFinishAuctColor, VolColor = volColor, VolBidColor = volBidColor, VolAskColor = volAskColor, VAColor = vAColor, VPColor = vPColor, AbsorptionBidColor = absorptionBidColor, AbsorptionAskColor = absorptionAskColor }, input, ref cacheiotecOFPlus4);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.lab.iotecOFPlus4 iotecOFPlus4(System.Windows.Media.Brush barUpColor, System.Windows.Media.Brush barDownColor, System.Windows.Media.Brush dojiColor, System.Windows.Media.Brush wickColor, System.Windows.Media.Brush pocColor, System.Windows.Media.Brush deltaPosColor, System.Windows.Media.Brush deltaNegColor, System.Windows.Media.Brush textColor, System.Windows.Media.Brush askImbColor, System.Windows.Media.Brush bidImbColor, System.Windows.Media.Brush stckAskImbColor, System.Windows.Media.Brush stckBidImbColor, System.Windows.Media.Brush unFinishAuctColor, System.Windows.Media.Brush volColor, System.Windows.Media.Brush volBidColor, System.Windows.Media.Brush volAskColor, System.Windows.Media.Brush vAColor, System.Windows.Media.Brush vPColor, System.Windows.Media.Brush absorptionBidColor, System.Windows.Media.Brush absorptionAskColor)
		{
			return indicator.iotecOFPlus4(Input, barUpColor, barDownColor, dojiColor, wickColor, pocColor, deltaPosColor, deltaNegColor, textColor, askImbColor, bidImbColor, stckAskImbColor, stckBidImbColor, unFinishAuctColor, volColor, volBidColor, volAskColor, vAColor, vPColor, absorptionBidColor, absorptionAskColor);
		}

		public Indicators.lab.iotecOFPlus4 iotecOFPlus4(ISeries<double> input , System.Windows.Media.Brush barUpColor, System.Windows.Media.Brush barDownColor, System.Windows.Media.Brush dojiColor, System.Windows.Media.Brush wickColor, System.Windows.Media.Brush pocColor, System.Windows.Media.Brush deltaPosColor, System.Windows.Media.Brush deltaNegColor, System.Windows.Media.Brush textColor, System.Windows.Media.Brush askImbColor, System.Windows.Media.Brush bidImbColor, System.Windows.Media.Brush stckAskImbColor, System.Windows.Media.Brush stckBidImbColor, System.Windows.Media.Brush unFinishAuctColor, System.Windows.Media.Brush volColor, System.Windows.Media.Brush volBidColor, System.Windows.Media.Brush volAskColor, System.Windows.Media.Brush vAColor, System.Windows.Media.Brush vPColor, System.Windows.Media.Brush absorptionBidColor, System.Windows.Media.Brush absorptionAskColor)
		{
			return indicator.iotecOFPlus4(input, barUpColor, barDownColor, dojiColor, wickColor, pocColor, deltaPosColor, deltaNegColor, textColor, askImbColor, bidImbColor, stckAskImbColor, stckBidImbColor, unFinishAuctColor, volColor, volBidColor, volAskColor, vAColor, vPColor, absorptionBidColor, absorptionAskColor);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.lab.iotecOFPlus4 iotecOFPlus4(System.Windows.Media.Brush barUpColor, System.Windows.Media.Brush barDownColor, System.Windows.Media.Brush dojiColor, System.Windows.Media.Brush wickColor, System.Windows.Media.Brush pocColor, System.Windows.Media.Brush deltaPosColor, System.Windows.Media.Brush deltaNegColor, System.Windows.Media.Brush textColor, System.Windows.Media.Brush askImbColor, System.Windows.Media.Brush bidImbColor, System.Windows.Media.Brush stckAskImbColor, System.Windows.Media.Brush stckBidImbColor, System.Windows.Media.Brush unFinishAuctColor, System.Windows.Media.Brush volColor, System.Windows.Media.Brush volBidColor, System.Windows.Media.Brush volAskColor, System.Windows.Media.Brush vAColor, System.Windows.Media.Brush vPColor, System.Windows.Media.Brush absorptionBidColor, System.Windows.Media.Brush absorptionAskColor)
		{
			return indicator.iotecOFPlus4(Input, barUpColor, barDownColor, dojiColor, wickColor, pocColor, deltaPosColor, deltaNegColor, textColor, askImbColor, bidImbColor, stckAskImbColor, stckBidImbColor, unFinishAuctColor, volColor, volBidColor, volAskColor, vAColor, vPColor, absorptionBidColor, absorptionAskColor);
		}

		public Indicators.lab.iotecOFPlus4 iotecOFPlus4(ISeries<double> input , System.Windows.Media.Brush barUpColor, System.Windows.Media.Brush barDownColor, System.Windows.Media.Brush dojiColor, System.Windows.Media.Brush wickColor, System.Windows.Media.Brush pocColor, System.Windows.Media.Brush deltaPosColor, System.Windows.Media.Brush deltaNegColor, System.Windows.Media.Brush textColor, System.Windows.Media.Brush askImbColor, System.Windows.Media.Brush bidImbColor, System.Windows.Media.Brush stckAskImbColor, System.Windows.Media.Brush stckBidImbColor, System.Windows.Media.Brush unFinishAuctColor, System.Windows.Media.Brush volColor, System.Windows.Media.Brush volBidColor, System.Windows.Media.Brush volAskColor, System.Windows.Media.Brush vAColor, System.Windows.Media.Brush vPColor, System.Windows.Media.Brush absorptionBidColor, System.Windows.Media.Brush absorptionAskColor)
		{
			return indicator.iotecOFPlus4(input, barUpColor, barDownColor, dojiColor, wickColor, pocColor, deltaPosColor, deltaNegColor, textColor, askImbColor, bidImbColor, stckAskImbColor, stckBidImbColor, unFinishAuctColor, volColor, volBidColor, volAskColor, vAColor, vPColor, absorptionBidColor, absorptionAskColor);
		}
	}
}

#endregion
