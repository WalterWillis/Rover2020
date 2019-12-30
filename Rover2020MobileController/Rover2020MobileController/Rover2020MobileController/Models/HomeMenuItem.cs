using System;
using System.Collections.Generic;
using System.Text;

namespace Rover2020MobileController.Models
{
    public enum MenuItemType
    {
        Browse,
        About,
        Control
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
