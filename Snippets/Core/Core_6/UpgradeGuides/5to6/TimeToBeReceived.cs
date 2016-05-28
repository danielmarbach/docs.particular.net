﻿// ReSharper disable RedundantAssignment
namespace Core6.UpgradeGuides._5to6
{
    using System;
    using NServiceBus.DeliveryConstraints;
    using NServiceBus.Performance.TimeToBeReceived;
    using NServiceBus.Pipeline;

    class TimeToBeReceived
    {
        TimeToBeReceived(IRoutingContext context)
        {
            #region SetDeliveryConstraintDiscardIfNotReceivedBefore
            var timeToBeReceived = TimeSpan.FromSeconds(25);
            context.Extensions.AddDeliveryConstraint(new DiscardIfNotReceivedBefore(timeToBeReceived));
            #endregion

            #region ReadDeliveryConstraintDiscardIfNotReceivedBefore
            DiscardIfNotReceivedBefore constraint;
            context.Extensions.TryGetDeliveryConstraint(out constraint);
            timeToBeReceived = constraint.MaxTime;
            #endregion
        }
    }
}
