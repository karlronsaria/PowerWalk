/*
 * Created by SharpDevelop.
 * User: Drew
 * Date: 4/16/2016
 * Time: 4:03 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace PowerWalk
{
    /// <summary>
    /// Description of Return.
    /// </summary>
    
    public class Return : Exception
    {
        public object payload;
        
        public Return(object newPayload)
        {
            Payload = newPayload;
        }
        
        public Return(): this(null) {}
        
        public object Payload
        {
            get { return payload;  }
            set { payload = value; }
        }
        
        public static Return Void()
        {
            return new Return();
        }
    }
}
