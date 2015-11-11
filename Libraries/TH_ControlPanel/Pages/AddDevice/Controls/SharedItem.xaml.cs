﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace TH_DeviceManager.Pages.AddDevice.Controls
{
    /// <summary>
    /// Interaction logic for SharedItem.xaml
    /// </summary>
    public partial class SharedItem : UserControl
    {
        public SharedItem()
        {
            InitializeComponent();
            DataContext = this;
        }

        public TH_Configuration.User.Management.SharedListItem listitem;


        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(ImageSource), typeof(SharedItem), new PropertyMetadata(null));

        
        public string Manufacturer
        {
            get { return (string)GetValue(ManufacturerProperty); }
            set { SetValue(ManufacturerProperty, value); }
        }

        public static readonly DependencyProperty ManufacturerProperty =
            DependencyProperty.Register("Manufacturer", typeof(string), typeof(SharedItem), new PropertyMetadata(null));



        public string DeviceType
        {
            get { return (string)GetValue(DeviceTypeProperty); }
            set { SetValue(DeviceTypeProperty, value); }
        }

        public static readonly DependencyProperty DeviceTypeProperty =
            DependencyProperty.Register("DeviceType", typeof(string), typeof(SharedItem), new PropertyMetadata(null));


        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(SharedItem), new PropertyMetadata(null));


        public string Model
        {
            get { return (string)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }

        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register("Model", typeof(string), typeof(SharedItem), new PropertyMetadata(null));



        public string Controller
        {
            get { return (string)GetValue(ControllerProperty); }
            set { SetValue(ControllerProperty, value); }
        }

        public static readonly DependencyProperty ControllerProperty =
            DependencyProperty.Register("Controller", typeof(string), typeof(SharedItem), new PropertyMetadata(null));


        public delegate void Clicked_Handler(SharedItem item);
        public event Clicked_Handler AddClicked;

        private void Add_Clicked(TH_WPF.Button_01 bt)
        {
            if (AddClicked != null) AddClicked(this);
        }



        
        
    }
}
