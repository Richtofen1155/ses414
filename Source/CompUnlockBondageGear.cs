﻿
using System;
using System.Linq;
using System.Collections.Generic;

using Verse;
using Verse.AI;
using RimWorld;

namespace rjw {
	public class CompUnlockBondageGear : CompUseEffect {
		public override float OrderPriority {
			get {
				return -69;
			}
		}

		public override void DoEffect (Pawn p)
		{
			base.DoEffect (p);
			var key_stamp = parent.GetComp<CompHoloCryptoStamped> ();

			if ((key_stamp == null) || (p.MapHeld == null) || (p.apparel == null))
				return;

			Apparel locked_app = null;
			var any_locked = false; {
				foreach (var app in p.apparel.WornApparel) {
					var app_stamp = app.GetComp<CompHoloCryptoStamped> ();
					if (app_stamp != null) {
						any_locked = true;
						if (app_stamp.matches (key_stamp)) {
							locked_app = app;
							break;
						}
					}
				}
			}

			if (locked_app != null) {
				//locked_app.Notify_Stripped (p); // TODO This was removed. Necessary?

				p.apparel.Remove (locked_app);
				Thing dropped = null;
				GenThing.TryDropAndSetForbidden (locked_app, p.Position, p.MapHeld, ThingPlaceMode.Near, out dropped, false);
				if (dropped != null)
					parent.Destroy (DestroyMode.Vanish);
				else if (PawnUtility.ShouldSendNotificationAbout (p))
					Messages.Message ("Couldn't drop " + locked_app.def.label, p, MessageSound.Negative);

			} else if (any_locked)
				Messages.Message ("The key doesn't fit!", p, MessageSound.Negative);
		}
	}
}
