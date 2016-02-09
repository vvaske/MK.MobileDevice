/*

 */
using System;
using Reiboot;
using rebootpwn;


namespace recoverysaver
{
    public class librecoverysaver
    {
    	public static rebootpwn.rebootpwn HijackIDevice()
        {
            rebootpwn.rebootpwn rbpwn = new rebootpwn.rebootpwn();
            //Console.WriteLine("Initializing");
            rbpwn.initRbthck();
            System.Threading.Thread.Sleep(200);
            int mode = rbpwn.connectionStatus;
            recoverydll.iphone_mode iphm = (recoverydll.iphone_mode)mode;
            //Console.WriteLine(mode);
            mode = rbpwn.connectionStatus;
            while (mode==0)
            {
                mode = rbpwn.connectionStatus;
            }
            switch (mode)
            {
                //current status
                case 0:
                case 3:
                case 4:
                    break;
                case 1:
                    //successful connect
                    break;
                case 2:
                    //Not connecting
                    break;
                case 5:
                    //In Recovery
                    break;
                case 6:
                    //Not in recovery
                    break;
                default:
                    //return;
                    break;
            }
            //Console.WriteLine("=======");
            //Console.WriteLine("1. Full Reboot");
            //Console.WriteLine("2. Enter Recovery");
            //Console.WriteLine("3. Exit Recovery");
            //string c = Console.ReadLine();
            return rbpwn;

            //rbpwn.EnterRecovery();
            //rbpwn.ExitRecovery();
            //Console.ReadKey(true);
        }
        public static void FullReboot(rebootpwn.rebootpwn rbpwn)
        {
        	rbpwn.EnterRecovery();
        	int mode = rbpwn.connectionStatus;
        	//Console.WriteLine("Waiting for device to boot into recovery...");
        	while (mode!=5)
            {
                mode = rbpwn.connectionStatus;
            }
        	rbpwn.ExitRecovery();
        }
    	public static bool EnterRecovery(rebootpwn.rebootpwn rbp)
    	{
    		return rbp.EnterRecovery();
    	}
    	public static bool ExitRecovery(rebootpwn.rebootpwn rbp)
    	{
    		return rbp.ExitRecovery();
    	}
        
    }
}