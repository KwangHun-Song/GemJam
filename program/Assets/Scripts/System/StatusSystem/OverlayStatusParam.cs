using System;

namespace OverlayStatusSystem {
    public class OverlayStatusParam {
        public object Value { get; set; }
        public OverlayStatusParam(object value) => this.Value = value;
    }
}