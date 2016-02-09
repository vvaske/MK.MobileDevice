using System;
//using System.Windows.Forms;
using System.Windows.Threading;

namespace MK.MobileDevice.XEDevice
{
	// Token: 0x0200001E RID: 30
	public class iPhoneXEManager
	{
		// Token: 0x060000F3 RID: 243 RVA: 0x0000211A File Offset: 0x0000031A
		public iPhoneXEManager(ConnectEventHandler myConnectHandler, ConnectEventHandler myDisconnectHandler)
		{
			this.Connect += myConnectHandler;
			this.Disconnect += myDisconnectHandler;
			this.doConstruction();
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x00002154 File Offset: 0x00000354
		private void DfuConnectCallback(ref AMRecoveryDevice callback)
		{
			this.OnDfuConnect(new DeviceNotificationEventArgs(callback));
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x00002167 File Offset: 0x00000367
		private void DfuDisconnectCallback(ref AMRecoveryDevice callback)
		{
			this.OnDfuDisconnect(new DeviceNotificationEventArgs(callback));
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x000066A0 File Offset: 0x000048A0
		private unsafe void doConstruction()
		{
			this.dnc = new DeviceNotificationCallback(this.NotifyCallback);
			this.drn1 = new DeviceRestoreNotificationCallback(this.DfuConnectCallback);
			this.drn2 = new DeviceRestoreNotificationCallback(this.RecoveryConnectCallback);
			this.drn3 = new DeviceRestoreNotificationCallback(this.DfuDisconnectCallback);
			this.drn4 = new DeviceRestoreNotificationCallback(this.RecoveryDisconnectCallback);
			try
			{
				void* ptr;
				int num = MobileDeviceBase.AMDeviceNotificationSubscribe(this.dnc, 0u, 0u, 0u, out ptr);
				if (num != 0)
				{
					throw new Exception("AMDeviceNotificationSubscribe failed with error " + num);
				}
				num = MobileDeviceBase.AMRestoreRegisterForDeviceNotifications(this.drn1, this.drn2, this.drn3, this.drn4, 0u, null);
				if (num != 0)
				{
					throw new Exception("AMRestoreRegisterForDeviceNotifications failed with error " + num);
				}
			}
			catch (Exception ex)
			{
                throw ex;
				//MessageBox.Show(LocalizedResources.txtUnableToStartIPhoneListner + "\r\n\r\n  Error Code: " + ex.Message.ToString());
			}
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x0000682C File Offset: 0x00004A2C
		private void NotifyCallback(ref AMDeviceNotificationCallbackInfo callback)
		{
			if (callback.msg == NotificationMessage.Connected)
			{
				this.OnConnect(new ConnectEventArgs(callback));
			}
			else if (callback.msg == NotificationMessage.Disconnected)
			{
				this.OnDisconnect(new ConnectEventArgs(callback));
			}
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x000067A8 File Offset: 0x000049A8
		public void NotifyUpdate(bool isFromDevToComp, string devID, string compPath, string devPath, string devService, double fraction)
		{
			if (this._safeUpdateDelegate != null && this._safeUpdateDispatcher != null)
			{
				this._safeUpdateDispatcher.BeginInvoke(new Action(delegate
				{
					this._safeUpdateDelegate.HandleTransferUpdate(isFromDevToComp, devID, compPath, devPath, devService, fraction);
				}), new object[0]);
			}
		}

		// Token: 0x060000FB RID: 251 RVA: 0x0000687C File Offset: 0x00004A7C
		protected void OnConnect(ConnectEventArgs args)
		{
			ConnectEventHandler connect = this.Connect;
			if (connect != null)
			{
				connect(this, args);
			}
		}

		// Token: 0x060000FD RID: 253 RVA: 0x000068C4 File Offset: 0x00004AC4
		protected void OnDfuConnect(DeviceNotificationEventArgs args)
		{
			EventHandler dfuConnect = this.DfuConnect;
			if (dfuConnect != null)
			{
				dfuConnect(this, args);
			}
		}

		// Token: 0x060000FE RID: 254 RVA: 0x000068E8 File Offset: 0x00004AE8
		protected void OnDfuDisconnect(DeviceNotificationEventArgs args)
		{
			EventHandler dfuDisconnect = this.DfuDisconnect;
			if (dfuDisconnect != null)
			{
				dfuDisconnect(this, args);
			}
		}

		// Token: 0x060000FC RID: 252 RVA: 0x000068A0 File Offset: 0x00004AA0
		protected void OnDisconnect(ConnectEventArgs args)
		{
			ConnectEventHandler disconnect = this.Disconnect;
			if (disconnect != null)
			{
				disconnect(this, args);
			}
		}

		// Token: 0x060000FF RID: 255 RVA: 0x0000690C File Offset: 0x00004B0C
		protected void OnRecoveryModeEnter(DeviceNotificationEventArgs args)
		{
			EventHandler recoveryModeEnter = this.RecoveryModeEnter;
			if (recoveryModeEnter != null)
			{
				recoveryModeEnter(this, args);
			}
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00006930 File Offset: 0x00004B30
		protected void OnRecoveryModeLeave(DeviceNotificationEventArgs args)
		{
			EventHandler recoveryModeLeave = this.RecoveryModeLeave;
			if (recoveryModeLeave != null)
			{
				recoveryModeLeave(this, args);
			}
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x0000217A File Offset: 0x0000037A
		private void RecoveryConnectCallback(ref AMRecoveryDevice callback)
		{
			this.OnRecoveryModeEnter(new DeviceNotificationEventArgs(callback));
		}

		// Token: 0x060000FA RID: 250 RVA: 0x0000218D File Offset: 0x0000038D
		private void RecoveryDisconnectCallback(ref AMRecoveryDevice callback)
		{
			this.OnRecoveryModeLeave(new DeviceNotificationEventArgs(callback));
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00002144 File Offset: 0x00000344
		public void SetSafeDelegate(Dispatcher disp, IDSafeUpdateDelegate del)
		{
			this._safeUpdateDispatcher = disp;
			this._safeUpdateDelegate = del;
		}

		// Token: 0x14000001 RID: 1
		// Token: 0x060000E6 RID: 230 RVA: 0x000063D0 File Offset: 0x000045D0
		// Token: 0x060000E7 RID: 231 RVA: 0x0000640C File Offset: 0x0000460C
		private event ConnectEventHandler Connect;

		// Token: 0x14000005 RID: 5
		// Token: 0x060000EE RID: 238 RVA: 0x000065B0 File Offset: 0x000047B0
		// Token: 0x060000EF RID: 239 RVA: 0x000065EC File Offset: 0x000047EC
		private event EventHandler DfuConnect;

		// Token: 0x14000003 RID: 3
		// Token: 0x060000EA RID: 234 RVA: 0x000064C0 File Offset: 0x000046C0
		// Token: 0x060000EB RID: 235 RVA: 0x000064FC File Offset: 0x000046FC
		private event EventHandler DfuDisconnect;

		// Token: 0x14000002 RID: 2
		// Token: 0x060000E8 RID: 232 RVA: 0x00006448 File Offset: 0x00004648
		// Token: 0x060000E9 RID: 233 RVA: 0x00006484 File Offset: 0x00004684
		private event ConnectEventHandler Disconnect;

		// Token: 0x14000004 RID: 4
		// Token: 0x060000EC RID: 236 RVA: 0x00006538 File Offset: 0x00004738
		// Token: 0x060000ED RID: 237 RVA: 0x00006574 File Offset: 0x00004774
		private event EventHandler RecoveryModeEnter;

		// Token: 0x14000006 RID: 6
		// Token: 0x060000F0 RID: 240 RVA: 0x00006628 File Offset: 0x00004828
		// Token: 0x060000F1 RID: 241 RVA: 0x00006664 File Offset: 0x00004864
		private event EventHandler RecoveryModeLeave;

		// Token: 0x04000083 RID: 131
		private DeviceNotificationCallback dnc;

		// Token: 0x04000084 RID: 132
		private DeviceRestoreNotificationCallback drn1;

		// Token: 0x04000085 RID: 133
		private DeviceRestoreNotificationCallback drn2;

		// Token: 0x04000086 RID: 134
		private DeviceRestoreNotificationCallback drn3;

		// Token: 0x04000087 RID: 135
		private DeviceRestoreNotificationCallback drn4;

		// Token: 0x0400008F RID: 143
		private IDSafeUpdateDelegate _safeUpdateDelegate = null;

		// Token: 0x0400008E RID: 142
		private Dispatcher _safeUpdateDispatcher = null;
	}
}
