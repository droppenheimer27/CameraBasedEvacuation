using System.Collections;

namespace CentralServer.Domain.CameraUpdates;

/// <summary>
/// Represents a collection of camera updates, providing functionality to calculate the number of people 
/// on-site based on the updates received from multiple cameras. The collection ensures that updates are 
/// processed correctly, even in the presence of delays or out-of-order updates. 
/// 
/// This class implements basic functionality for filtering, ordering, and deduplicating camera updates,
/// and calculates the net count of people on-site by summing the number of people entering and exiting
/// the premises based on camera data.
/// </summary>
public class CameraUpdatesCollection : ICameraUpdatesCollection
{
    private readonly IEnumerable<CameraUpdate> _cameraUpdates;

    public CameraUpdatesCollection(IEnumerable<CameraUpdate> cameraUpdates)
    {
        _cameraUpdates = cameraUpdates ?? throw new ArgumentNullException(nameof(cameraUpdates));
    }

    public IEnumerator<CameraUpdate> GetEnumerator() => _cameraUpdates.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => _cameraUpdates.Count();

    /// <summary>
    /// Calculates the current number of people on-site based on the camera updates, filtered by the provided timestamp.
    /// 
    /// This method filters the updates to only include those with a timestamp less than or equal to the provided 
    /// <paramref name="latestTimestamp"/>. It then removes any duplicate updates for the same camera at the same time
    /// and processes the updates in chronological order to ensure the correct aggregation of people count, even if updates 
    /// arrive out of order or with delays. The method calculates the net count of people on-site by summing the entries 
    /// and exits, ensuring that any updates with a timestamp beyond the provided <paramref name="latestTimestamp"/> are excluded.
    /// 
    /// Assumptions:
    /// - Zones covered by cameras are distinct, without overlap. In case of overlapping zones, additional solutions like 
    ///   tracking unique IDs or using machine learning for person detection are required to avoid double-counting.
    /// </summary>
    /// <param name="latestTimestamp">The latest timestamp to consider for the calculation, excluding updates beyond this time.</param>
    /// <returns>The total count of people on-site.</returns>
    public int CurrentCountPeopleOnSite(DateTime latestTimestamp) 
        => _cameraUpdates
            .Where(update => update.Timestamp <= latestTimestamp)
            .OrderBy(update => update.Timestamp)
            .DistinctBy(update => new { update.CameraId, update.Timestamp, update.In, update.Out })
            .Aggregate(0, CalculatePeopleCount);

    private int CalculatePeopleCount(int total, CameraUpdate update) 
        => total + (update.In - update.Out);
}