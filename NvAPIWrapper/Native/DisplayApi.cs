using System;
using System.Collections.Generic;
using System.Linq;
using NvAPIWrapper.Native.Display;
using NvAPIWrapper.Native.Display.Structures;
using NvAPIWrapper.Native.Exceptions;
using NvAPIWrapper.Native.General;
using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.Helpers;
using NvAPIWrapper.Native.Helpers.Structures;
using NvAPIWrapper.Native.Interfaces.Display;
using NvAPIWrapper.Native.Interfaces.GPU;

namespace NvAPIWrapper.Native
{
    /// <summary>
    ///     Contains display and display control static functions
    /// </summary>
    public static class DisplayApi
    {
        /// <summary>
        ///     This function converts the unattached display handle to an active attached display handle.
        ///     At least one GPU must be present in the system and running an NVIDIA display driver.
        /// </summary>
        /// <param name="display">An unattached display handle to convert.</param>
        /// <returns>Display handle of newly created display.</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: Invalid UnAttachedDisplayHandle handle.</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static DisplayHandle CreateDisplayFromUnAttachedDisplay(UnAttachedDisplayHandle display)
        {
            var createDisplayFromUnAttachedDisplay =
                DelegateFactory.GetDelegate<Delegates.Display.NvAPI_CreateDisplayFromUnAttachedDisplay>();
            var status = createDisplayFromUnAttachedDisplay(display, out var newDisplay);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return newDisplay;
        }

        /// <summary>
        ///     This function returns the handle of all NVIDIA displays
        ///     Note: Display handles can get invalidated on a mode-set, so the calling applications need to re-enum the handles
        ///     after every mode-set.
        /// </summary>
        /// <returns>Array of display handles.</returns>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA device found in the system</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static DisplayHandle[] EnumNvidiaDisplayHandle()
        {
            var enumNvidiaDisplayHandle =
                DelegateFactory.GetDelegate<Delegates.Display.NvAPI_EnumNvidiaDisplayHandle>();
            var results = new List<DisplayHandle>();
            uint i = 0;

            while (true)
            {
                var status = enumNvidiaDisplayHandle(i, out var displayHandle);

                if (status == Status.EndEnumeration)
                {
                    break;
                }

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                results.Add(displayHandle);
                i++;
            }

            return results.ToArray();
        }

        /// <summary>
        ///     This function returns the handle of all unattached NVIDIA displays
        ///     Note: Display handles can get invalidated on a mode-set, so the calling applications need to re-enum the handles
        ///     after every mode-set.
        /// </summary>
        /// <returns>Array of unattached display handles.</returns>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA device found in the system</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static UnAttachedDisplayHandle[] EnumNvidiaUnAttachedDisplayHandle()
        {
            var enumNvidiaUnAttachedDisplayHandle =
                DelegateFactory.GetDelegate<Delegates.Display.NvAPI_EnumNvidiaUnAttachedDisplayHandle>();
            var results = new List<UnAttachedDisplayHandle>();
            uint i = 0;

            while (true)
            {
                var status = enumNvidiaUnAttachedDisplayHandle(i, out var displayHandle);

                if (status == Status.EndEnumeration)
                {
                    break;
                }

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                results.Add(displayHandle);
                i++;
            }

            return results.ToArray();
        }

        /// <summary>
        ///     This function gets the active outputId associated with the display handle.
        /// </summary>
        /// <param name="display">
        ///     NVIDIA Display selection. It can be DisplayHandle.DefaultHandle or a handle enumerated from
        ///     DisplayApi.EnumNVidiaDisplayHandle().
        /// </param>
        /// <returns>
        ///     The active display output ID associated with the selected display handle hNvDisplay. The output id will have
        ///     only one bit set. In the case of Clone or Span mode, this will indicate the display outputId of the primary display
        ///     that the GPU is driving.
        /// </returns>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found.</exception>
        /// <exception cref="NVIDIAApiException">Status.ExpectedDisplayHandle: display is not a valid display handle.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static OutputId GetAssociatedDisplayOutputId(DisplayHandle display)
        {
            var getAssociatedDisplayOutputId =
                DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetAssociatedDisplayOutputId>();
            var status = getAssociatedDisplayOutputId(display, out var outputId);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return outputId;
        }

        /// <summary>
        ///     This function returns the handle of the NVIDIA display that is associated with the given display "name" (such as
        ///     "\\.\DISPLAY1").
        /// </summary>
        /// <param name="name">Display name</param>
        /// <returns>Display handle of associated display</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: Display name is null.</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA device maps to that display name.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static DisplayHandle GetAssociatedNvidiaDisplayHandle(string name)
        {
            var getAssociatedNvidiaDisplayHandle =
                DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetAssociatedNvidiaDisplayHandle>();
            var status = getAssociatedNvidiaDisplayHandle(name, out var display);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return display;
        }

        /// <summary>
        ///     For a given NVIDIA display handle, this function returns a string (such as "\\.\DISPLAY1") to identify the display.
        /// </summary>
        /// <param name="display">Handle of the associated display</param>
        /// <returns>Name of the display</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: Display handle is null.</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA device maps to that display name.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static string GetAssociatedNvidiaDisplayName(DisplayHandle display)
        {
            var getAssociatedNvidiaDisplayName =
                DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetAssociatedNvidiaDisplayName>();
            var status = getAssociatedNvidiaDisplayName(display, out var displayName);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return displayName.Value;
        }

        /// <summary>
        ///     This function returns the handle of an unattached NVIDIA display that is associated with the given display "name"
        ///     (such as "\\DISPLAY1").
        /// </summary>
        /// <param name="name">Display name</param>
        /// <returns>Display handle of associated unattached display</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: Display name is null.</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA device maps to that display name.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static UnAttachedDisplayHandle GetAssociatedUnAttachedNvidiaDisplayHandle(string name)
        {
            var getAssociatedUnAttachedNvidiaDisplayHandle =
                DelegateFactory.GetDelegate<Delegates.Display.NvAPI_DISP_GetAssociatedUnAttachedNvidiaDisplayHandle>();
            var status = getAssociatedUnAttachedNvidiaDisplayHandle(name, out var display);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return display;
        }

        /// <summary>
        ///     This function returns color information relating to the targeted display id.
        /// </summary>
        /// <param name="displayId">The targeted display output id.</param>
        /// <returns>Data corresponding to color information</returns>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static TColorData GetColorData<TColorData>(uint displayId) where TColorData : struct, IColorData
        {
            var colorData = (TColorData)Activator.CreateInstance(typeof(TColorData), ColorCommand.Get);

            return ColorControl(displayId, colorData);
        }

        /// <summary>
        ///     This function returns the default color information of the targeted display id.
        /// </summary>
        /// <param name="displayId">The targeted display output id.</param>
        /// <returns>Data corresponding to default color information</returns>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static TColorData GetDefaultColorData<TColorData>(uint displayId) where TColorData : struct, IColorData
        {
            var colorData = (TColorData)Activator.CreateInstance(typeof(TColorData), ColorCommand.GetDefault);

            return ColorControl(displayId, colorData);
        }

        /// <summary>
        ///     This API lets caller retrieve the current global display configuration.
        ///     Note: User should dispose all returned PathInfo objects
        /// </summary>
        /// <returns>Array of path information</returns>
        /// <exception cref="NVIDIANotSupportedException">This operation is not supported.</exception>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: Invalid input parameter.</exception>
        /// <exception cref="NVIDIAApiException">Status.DeviceBusy: ModeSet has not yet completed. Please wait and call it again.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static IPathInfo[] GetDisplayConfig()
        {
            var getDisplayConfig = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_DISP_GetDisplayConfig>();
            uint allAvailable = 0;
            var status = getDisplayConfig(ref allAvailable, ValueTypeArray.Null);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            if (allAvailable == 0)
            {
                return new IPathInfo[0];
            }

            foreach (var acceptType in getDisplayConfig.Accepts())
            {
                var count = allAvailable;
                var instances = acceptType.Instantiate<IPathInfo>().Repeat((int) allAvailable);

                using (var pathInfos = ValueTypeArray.FromArray(instances, acceptType))
                {
                    status = getDisplayConfig(ref count, pathInfos);

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    instances = pathInfos.ToArray<IPathInfo>((int) count, acceptType);
                }

                if (instances.Length <= 0)
                {
                    return new IPathInfo[0];
                }

                // After allocation, we should make sure to dispose objects
                // In this case however, the responsibility is on the user shoulders
                instances = instances.AllocateAll().ToArray();

                using (var pathInfos = ValueTypeArray.FromArray(instances, acceptType))
                {
                    status = getDisplayConfig(ref count, pathInfos);

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    return pathInfos.ToArray<IPathInfo>((int) count, acceptType);
                }
            }

            throw new NVIDIANotSupportedException("This operation is not supported.");
        }

        /// <summary>
        ///     Gets the build title of the Driver Settings Database for a display
        /// </summary>
        /// <param name="displayHandle">The display handle to get DRS build title.</param>
        /// <returns>The DRS build title.</returns>
        public static string GetDisplayDriverBuildTitle(DisplayHandle displayHandle)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetDisplayDriverBuildTitle>()(displayHandle,
                    out var name);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return name.Value;
        }

        /// <summary>
        ///     This function retrieves the available driver memory footprint for the GPU associated with a display.
        /// </summary>
        /// <param name="displayHandle">Handle of the display for which the memory information of its GPU is to be extracted.</param>
        /// <returns>The memory footprint available in the driver.</returns>
        /// <exception cref="NVIDIANotSupportedException">This operation is not supported.</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static IDisplayDriverMemoryInfo GetDisplayDriverMemoryInfo(DisplayHandle displayHandle)
        {
            var getMemoryInfo = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetDisplayDriverMemoryInfo>();

            foreach (var acceptType in getMemoryInfo.Accepts())
            {
                var instance = acceptType.Instantiate<IDisplayDriverMemoryInfo>();

                using (var displayDriverMemoryInfo = ValueTypeReference.FromValueType(instance, acceptType))
                {
                    var status = getMemoryInfo(displayHandle, displayDriverMemoryInfo);

                    if (status == Status.IncompatibleStructureVersion)
                    {
                        continue;
                    }

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    return displayDriverMemoryInfo.ToValueType<IDisplayDriverMemoryInfo>(acceptType);
                }
            }

            throw new NVIDIANotSupportedException("This operation is not supported.");
        }

        /// <summary>
        ///     This API retrieves the Display Id of a given display by display name. The display must be active to retrieve the
        ///     displayId. In the case of clone mode or Surround gaming, the primary or top-left display will be returned.
        /// </summary>
        /// <param name="displayName">Name of display (Eg: "\\DISPLAY1" to retrieve the displayId for.</param>
        /// <returns>Display ID of the requested display.</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: One or more args passed in are invalid.</exception>
        /// <exception cref="NVIDIAApiException">Status.ApiNotInitialized: The NvAPI API needs to be initialized first</exception>
        /// <exception cref="NVIDIAApiException">Status.NoImplementation: This entry-point not available</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static uint GetDisplayIdByDisplayName(string displayName)
        {
            var getDisplayIdByDisplayName =
                DelegateFactory.GetDelegate<Delegates.Display.NvAPI_DISP_GetDisplayIdByDisplayName>();
            var status = getDisplayIdByDisplayName(displayName, out var display);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return display;
        }

        /// [PRIVATE]
        /// <summary>
        ///     This API returns the current saturation level from the Digital Vibrance Control
        /// </summary>
        /// <param name="display">
        ///     The targeted display's handle.
        /// </param>
        /// <returns>An instance of the PrivateDisplayDVCInfo structure containing requested information.</returns>
        public static PrivateDisplayDVCInfo GetDVCInfo(DisplayHandle display)
        {
            var instance = typeof(PrivateDisplayDVCInfo).Instantiate<PrivateDisplayDVCInfo>();

            using (var displayDVCInfoReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetDVCInfo>()(
                    display,
                    OutputId.Invalid,
                    displayDVCInfoReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return displayDVCInfoReference.ToValueType<PrivateDisplayDVCInfo>().GetValueOrDefault();
            }
        }

        /// [PRIVATE]
        /// <summary>
        ///     This API returns the current saturation level from the Digital Vibrance Control
        /// </summary>
        /// <param name="displayId">
        ///     The targeted display output id.
        /// </param>
        /// <returns>An instance of the PrivateDisplayDVCInfo structure containing requested information.</returns>
        public static PrivateDisplayDVCInfo GetDVCInfo(OutputId displayId)
        {
            var instance = typeof(PrivateDisplayDVCInfo).Instantiate<PrivateDisplayDVCInfo>();

            using (var displayDVCInfoReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetDVCInfo>()(
                    DisplayHandle.DefaultHandle,
                    displayId,
                    displayDVCInfoReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return displayDVCInfoReference.ToValueType<PrivateDisplayDVCInfo>().GetValueOrDefault();
            }
        }

        /// [PRIVATE]
        /// <summary>
        ///     This API returns the current and the default saturation level from the Digital Vibrance Control.
        ///     The difference between this API and the 'GetDVCInfo()' includes the possibility to get the default
        ///     saturation level as well as to query under saturated configurations.
        /// </summary>
        /// <param name="display">
        ///     The targeted display's handle.
        /// </param>
        /// <returns>An instance of the PrivateDisplayDVCInfoEx structure containing requested information.</returns>
        public static PrivateDisplayDVCInfoEx GetDVCInfoEx(DisplayHandle display)
        {
            var instance = typeof(PrivateDisplayDVCInfoEx).Instantiate<PrivateDisplayDVCInfoEx>();

            using (var displayDVCInfoReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetDVCInfoEx>()(
                    display,
                    OutputId.Invalid,
                    displayDVCInfoReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return displayDVCInfoReference.ToValueType<PrivateDisplayDVCInfoEx>().GetValueOrDefault();
            }
        }

        /// <summary>
        ///     This API returns the Display ID of the GDI Primary.
        /// </summary>
        /// <returns>Display ID of the GDI Primary.</returns>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: GDI Primary not on an NVIDIA GPU.</exception>
        /// <exception cref="NVIDIAApiException">Status.ApiNotInitialized: The NvAPI API needs to be initialized first</exception>
        /// <exception cref="NVIDIAApiException">Status.NoImplementation: This entry-point not available</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static uint GetGDIPrimaryDisplayId()
        {
            var getGDIPrimaryDisplay = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_DISP_GetGDIPrimaryDisplayId>();

            var status = getGDIPrimaryDisplay(out var displayId);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return displayId;
        }

        /// [PRIVATE]
        /// <summary>
        ///     This API returns the current and the default saturation level from the Digital Vibrance Control.
        ///     The difference between this API and the 'GetDVCInfo()' includes the possibility to get the default
        ///     saturation level as well as to query under saturated configurations.
        /// </summary>
        /// <param name="displayId">
        ///     The targeted display output id.
        /// </param>
        /// <returns>An instance of the PrivateDisplayDVCInfoEx structure containing requested information.</returns>
        public static PrivateDisplayDVCInfoEx GetDVCInfoEx(OutputId displayId)
        {
            var instance = typeof(PrivateDisplayDVCInfoEx).Instantiate<PrivateDisplayDVCInfoEx>();

            using (var displayDVCInfoReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetDVCInfoEx>()(
                    DisplayHandle.DefaultHandle,
                    displayId,
                    displayDVCInfoReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return displayDVCInfoReference.ToValueType<PrivateDisplayDVCInfoEx>().GetValueOrDefault();
            }
        }

        /// <summary>
        ///     This function returns HDR configuration data relating to the targeted display id.
        ///     Note: MasteringDisplayData values will be zero if HDRMode is Off.
        /// </summary>
        /// <param name="displayId">The targeted display output id.</param>
        /// <returns>HDR configuration data</returns>
        public static THDRColorData GetHDRColorData<THDRColorData>(uint displayId) where THDRColorData : struct, IHDRColorData
        {
            var hdrColorData = (THDRColorData)Activator.CreateInstance(typeof(THDRColorData), HDRCommand.Get);

            return HDRColorControl(displayId, hdrColorData);
        }

        /// <summary>
        ///     This API queries current state of one of the various scan-out composition parameters on the specified display.
        /// </summary>
        /// <param name="displayId">Combined physical display and GPU identifier of the display to query the configuration.</param>
        /// <param name="parameter">Scan-out composition parameter to by queried.</param>
        /// <param name="container">Additional container containing the returning data associated with the specified parameter.</param>
        /// <returns>Scan-out composition parameter value.</returns>
        public static ScanOutCompositionParameterValue GetScanOutCompositionParameter(
            uint displayId,
            ScanOutCompositionParameter parameter,
            out float container)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GPU_GetScanOutCompositionParameter>()(
                displayId,
                parameter,
                out var parameterValue,
                out container
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return parameterValue;
        }

        /// <summary>
        ///     This API queries the desktop and scan-out portion of the specified display.
        /// </summary>
        /// <param name="displayId">Combined physical display and GPU identifier of the display to query the configuration.</param>
        /// <returns>Desktop area to displayId mapping information.</returns>
        public static ScanOutInformationV1 GetScanOutConfiguration(uint displayId)
        {
            var instance = typeof(ScanOutInformationV1).Instantiate<ScanOutInformationV1>();

            using (var scanOutInformationReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GPU_GetScanOutConfigurationEx>()(
                    displayId,
                    scanOutInformationReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return scanOutInformationReference.ToValueType<ScanOutInformationV1>().GetValueOrDefault();
            }
        }

        /// <summary>
        ///     This API queries the desktop and scan-out portion of the specified display.
        /// </summary>
        /// <param name="displayId">Combined physical display and GPU identifier of the display to query the configuration.</param>
        /// <param name="desktopRectangle">Desktop area of the display in desktop coordinates.</param>
        /// <param name="scanOutRectangle">Scan-out area of the display relative to desktopRect.</param>
        public static void GetScanOutConfiguration(
            uint displayId,
            out Rectangle desktopRectangle,
            out Rectangle scanOutRectangle)
        {
            var instance1 = typeof(Rectangle).Instantiate<Rectangle>();
            var instance2 = typeof(Rectangle).Instantiate<Rectangle>();

            using (var desktopRectangleReference = ValueTypeReference.FromValueType(instance1))
            {
                using (var scanOutRectangleReference = ValueTypeReference.FromValueType(instance2))
                {
                    var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GPU_GetScanOutConfiguration>()(
                        displayId,
                        desktopRectangleReference,
                        scanOutRectangleReference
                    );

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    desktopRectangle = desktopRectangleReference.ToValueType<Rectangle>().GetValueOrDefault();
                    scanOutRectangle = scanOutRectangleReference.ToValueType<Rectangle>().GetValueOrDefault();
                }
            }
        }

        /// <summary>
        ///     This API queries current state of the intensity feature on the specified display.
        /// </summary>
        /// <param name="displayId">Combined physical display and GPU identifier of the display to query the configuration.</param>
        /// <returns>Intensity state data.</returns>
        public static ScanOutIntensityStateV1 GetScanOutIntensityState(uint displayId)
        {
            var instance = typeof(ScanOutIntensityStateV1).Instantiate<ScanOutIntensityStateV1>();

            using (var scanOutIntensityReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GPU_GetScanOutIntensityState>()(
                    displayId,
                    scanOutIntensityReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return scanOutIntensityReference.ToValueType<ScanOutIntensityStateV1>().GetValueOrDefault();
            }
        }

        /// <summary>
        ///     This API queries current state of the warping feature on the specified display.
        /// </summary>
        /// <param name="displayId">Combined physical display and GPU identifier of the display to query the configuration.</param>
        /// <returns>The warping state data.</returns>
        public static ScanOutWarpingStateV1 GetScanOutWarpingState(uint displayId)
        {
            var instance = typeof(ScanOutWarpingStateV1).Instantiate<ScanOutWarpingStateV1>();

            using (var scanOutWarpingReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GPU_GetScanOutWarpingState>()(
                    displayId,
                    scanOutWarpingReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return scanOutWarpingReference.ToValueType<ScanOutWarpingStateV1>().GetValueOrDefault();
            }
        }

        /// <summary>
        ///     This API lets caller enumerate all the supported NVIDIA display views - nView and DualView modes.
        /// </summary>
        /// <param name="display">
        ///     NVIDIA Display selection. It can be DisplayHandle.DefaultHandle or a handle enumerated from
        ///     DisplayApi.EnumNVidiaDisplayHandle().
        /// </param>
        /// <returns>Array of supported views.</returns>
        /// <exception cref="NVIDIANotSupportedException">This operation is not supported.</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred</exception>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: Invalid input parameter.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static TargetViewMode[] GetSupportedViews(DisplayHandle display)
        {
            var getSupportedViews = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetSupportedViews>();
            uint allAvailable = 0;
            var status = getSupportedViews(display, ValueTypeArray.Null, ref allAvailable);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            if (allAvailable == 0)
            {
                return new TargetViewMode[0];
            }

            if (!getSupportedViews.Accepts().Contains(typeof(TargetViewMode)))
            {
                throw new NVIDIANotSupportedException("This operation is not supported.");
            }

            using (
                var viewModes =
                    ValueTypeArray.FromArray(TargetViewMode.Standard.Repeat((int) allAvailable).Cast<object>(),
                        typeof(TargetViewMode).GetEnumUnderlyingType()))
            {
                status = getSupportedViews(display, viewModes, ref allAvailable);

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return viewModes.ToArray<TargetViewMode>((int) allAvailable,
                    typeof(TargetViewMode).GetEnumUnderlyingType());
            }
        }

        /// <summary>
        ///     This function returns the display name given, for example, "\\DISPLAY1", using the unattached NVIDIA display handle
        /// </summary>
        /// <param name="display">Handle of the associated unattached display</param>
        /// <returns>Name of the display</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: Display handle is null.</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA device maps to that display name.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static string GetUnAttachedAssociatedDisplayName(UnAttachedDisplayHandle display)
        {
            var getUnAttachedAssociatedDisplayName =
                DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetUnAttachedAssociatedDisplayName>();
            var status = getUnAttachedAssociatedDisplayName(display, out var displayName);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return displayName.Value;
        }

        /// <summary>
        ///     This function returns a boolean indicating if the targeted display supports the provided color settings.
        /// </summary>
        /// <param name="displayId">The targeted display output id.</param>
        /// <returns>true, if the color settings are supported, otherwise false</returns>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static bool IsSupportedColor<TColorData>(uint displayId, TColorData colorData) where TColorData : struct, IColorData
        {
            try
            {
                colorData.ColorCommand = ColorCommand.IsSupportedColor;
                _ = ColorControl(displayId, colorData);

                return true;
            }
            catch (NVIDIAApiException ex)
            {
                if (ex.Status == Status.NotSupported)
                    return false;

                throw;
            }
        }

        /// <summary>
        ///     This function applies color settings to the targeted display.
        /// </summary>
        /// <param name="displayId">The targeted display output id.</param>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static void SetColorData<TColorData>(uint displayId, TColorData colorData) where TColorData : struct, IColorData
        {
            colorData.ColorCommand = ColorCommand.Set;

            ColorControl(displayId, colorData);
        }

        /// <summary>
        ///     This API lets caller apply a global display configuration across multiple GPUs.
        ///     If all sourceIds are zero, then NvAPI will pick up sourceId's based on the following criteria :
        ///     - If user provides SourceModeInfo then we are trying to assign 0th SourceId always to GDIPrimary.
        ///     This is needed since active windows always moves along with 0th sourceId.
        ///     - For rest of the paths, we are incrementally assigning the SourceId per adapter basis.
        ///     - If user doesn't provide SourceModeInfo then NVAPI just picks up some default SourceId's in incremental order.
        ///     Note : NVAPI will not intelligently choose the SourceIDs for any configs that does not need a mode-set.
        /// </summary>
        /// <param name="pathInfos">Array of path information</param>
        /// <param name="flags">Flags for applying settings</param>
        /// <exception cref="NVIDIANotSupportedException">This operation is not supported.</exception>
        /// <exception cref="NVIDIAApiException">Status.ApiNotInitialized: NVAPI not initialized</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred</exception>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: Invalid input parameter.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static void SetDisplayConfig(IPathInfo[] pathInfos, DisplayConfigFlags flags)
        {
            var setDisplayConfig = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_DISP_SetDisplayConfig>();

            if (pathInfos.Length > 0 && !setDisplayConfig.Accepts().Contains(pathInfos[0].GetType()))
            {
                throw new NVIDIANotSupportedException("This operation is not supported.");
            }

            using (var arrayReference = ValueTypeArray.FromArray(pathInfos))
            {
                var status = setDisplayConfig((uint) pathInfos.Length, arrayReference, flags);

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }

        /// [PRIVATE]
        /// <summary>
        ///     This API sets the current saturation level for the Digital Vibrance Control
        /// </summary>
        /// <param name="display">
        ///     The targeted display's handle.
        /// </param>
        /// <param name="currentLevel">
        ///     The saturation level to be set.
        /// </param>
        public static void SetDVCLevel(DisplayHandle display, int currentLevel)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_SetDVCLevel>()(
                display,
                OutputId.Invalid,
                currentLevel
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// [PRIVATE]
        /// <summary>
        ///     This API sets the current saturation level for the Digital Vibrance Control
        /// </summary>
        /// <param name="displayId">
        ///     The targeted display output id.
        /// </param>
        /// <param name="currentLevel">
        ///     The saturation level to be set.
        /// </param>
        public static void SetDVCLevel(OutputId displayId, int currentLevel)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_SetDVCLevel>()(
                DisplayHandle.DefaultHandle,
                displayId,
                currentLevel
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// [PRIVATE]
        /// <summary>
        ///     This API sets the current saturation level for the Digital Vibrance Control.
        ///     The difference between this API and the 'SetDVCLevel()' includes the possibility to set under saturated
        ///     levels.
        /// </summary>
        /// <param name="display">
        ///     The targeted display's handle.
        /// </param>
        /// <param name="currentLevel">
        ///     The saturation level to be set.
        /// </param>
        public static void SetDVCLevelEx(DisplayHandle display, int currentLevel)
        {
            var instance = new PrivateDisplayDVCInfoEx(currentLevel);

            using (var displayDVCInfoReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_SetDVCLevelEx>()(
                    display,
                    OutputId.Invalid,
                    displayDVCInfoReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }

        /// [PRIVATE]
        /// <summary>
        ///     This API sets the current saturation level for the Digital Vibrance Control.
        ///     The difference between this API and the 'SetDVCLevel()' includes the possibility to set under saturated
        ///     levels.
        /// </summary>
        /// <param name="displayId">
        ///     The targeted display output id.
        /// </param>
        /// <param name="currentLevel">
        ///     The saturation level to be set.
        /// </param>
        public static void SetDVCLevelEx(OutputId displayId, int currentLevel)
        {
            var instance = new PrivateDisplayDVCInfoEx(currentLevel);

            using (var displayDVCInfoReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_SetDVCLevelEx>()(
                    DisplayHandle.DefaultHandle,
                    displayId,
                    displayDVCInfoReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }

        /// <summary>
        ///     This function applies HDR configuration to the targeted display id.
        ///     Note: As of NVAPI R410:
        ///       - setting the DynamicRange does not appear to not take effect
        ///       - setting the ColorFormat does not appear to not take effect
        ///       - setting the BPC only takes effect when switching HDRMode to On from Off, and only if
        ///         the ColorSelectionPolicy is not set to User (use the GetColorData function to determine the active ColorSelectionPolicy)
        ///     Note: Filling MasteringDisplayData values is not required for enabling HDRMode.
        ///     Note: Use the SetColorData function to better control BPC, and to control DynamicRange and ColorFormat.
        /// </summary>
        /// <param name="displayId">The targeted display output id.</param>
        /// <param name="hdrColorData">HDR configuration data</param>
        public static void SetHDRColorData<THDRColorData>(uint displayId, THDRColorData hdrColorData) where THDRColorData : struct, IHDRColorData
        {
            hdrColorData.HDRCommand = HDRCommand.Set;

            HDRColorControl(displayId, hdrColorData);
        }

        /// <summary>
        ///     This API sets various parameters that configure the scan-out composition feature on the specified display.
        /// </summary>
        /// <param name="displayId">Combined physical display and GPU identifier of the display to apply the intensity control.</param>
        /// <param name="parameter">The scan-out composition parameter to be set.</param>
        /// <param name="parameterValue">The value to be set for the specified parameter.</param>
        /// <param name="container">Additional container for data associated with the specified parameter.</param>
        // ReSharper disable once TooManyArguments
        public static void SetScanOutCompositionParameter(
            uint displayId,
            ScanOutCompositionParameter parameter,
            ScanOutCompositionParameterValue parameterValue,
            ref float container)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GPU_SetScanOutCompositionParameter>()(
                displayId,
                parameter,
                parameterValue,
                ref container
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API enables and sets up per-pixel intensity feature on the specified display.
        /// </summary>
        /// <param name="displayId">Combined physical display and GPU identifier of the display to apply the intensity control.</param>
        /// <param name="scanOutIntensity">The intensity texture info.</param>
        /// <param name="isSticky">Indicates whether the settings will be kept over a reboot.</param>
        public static void SetScanOutIntensity(uint displayId, IScanOutIntensity scanOutIntensity, out bool isSticky)
        {
            Status status;
            int isStickyInt;

            if (scanOutIntensity == null)
            {
                status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GPU_SetScanOutIntensity>()(
                    displayId,
                    ValueTypeReference.Null,
                    out isStickyInt
                );
            }
            else
            {
                using (var scanOutWarpingReference =
                    ValueTypeReference.FromValueType(scanOutIntensity, scanOutIntensity.GetType()))
                {
                    status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GPU_SetScanOutIntensity>()(
                        displayId,
                        scanOutWarpingReference,
                        out isStickyInt
                    );
                }
            }

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            isSticky = isStickyInt > 0;
        }

        /// <summary>
        ///     This API enables and sets up the warping feature on the specified display.
        /// </summary>
        /// <param name="displayId">Combined physical display and GPU identifier of the display to apply the intensity control.</param>
        /// <param name="scanOutWarping">The warping data info.</param>
        /// <param name="maximumNumberOfVertices">The maximum number of vertices.</param>
        /// <param name="isSticky">Indicates whether the settings will be kept over a reboot.</param>
        // ReSharper disable once TooManyArguments
        public static void SetScanOutWarping(
            uint displayId,
            ScanOutWarpingV1? scanOutWarping,
            ref int maximumNumberOfVertices,
            out bool isSticky)
        {
            Status status;
            int isStickyInt;

            if (scanOutWarping == null || maximumNumberOfVertices == 0)
            {
                maximumNumberOfVertices = 0;
                status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GPU_SetScanOutWarping>()(
                    displayId,
                    ValueTypeReference.Null,
                    ref maximumNumberOfVertices,
                    out isStickyInt
                );
            }
            else
            {
                using (var scanOutWarpingReference = ValueTypeReference.FromValueType(scanOutWarping.Value))
                {
                    status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GPU_SetScanOutWarping>()(
                        displayId,
                        scanOutWarpingReference,
                        ref maximumNumberOfVertices,
                        out isStickyInt
                    );
                }
            }

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            isSticky = isStickyInt > 0;
        }

        /// <summary>
        ///     This API controls the Color values.
        /// </summary>
        /// <param name="displayId">The targeted display output id.</param>
        /// <param name="colorData">Contains data corresponding to color information</param>
        /// <returns>Data corresponding to color information</returns>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        private static TColorData ColorControl<TColorData>(uint displayId, TColorData colorData) where TColorData : struct, IColorData
        {
            var colorControl = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_Disp_ColorControl>();

            using (var colorDataReference = ValueTypeReference.FromValueType(colorData))
            {
                var status = colorControl(displayId, colorDataReference);

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return colorDataReference.ToValueType<TColorData>().GetValueOrDefault();
            }
        }

        /// <summary>
        ///     This API configures High Dynamic Range (HDR) and Extended Dynamic Range (EDR) output.
        /// </summary>
        /// <param name="displayId">The targeted display output id.</param>
        /// <param name="hdrColorData">HDR configuration data</param>
        /// <returns>HDR configuration data</returns>
        public static THDRColorData HDRColorControl<THDRColorData>(uint displayId, THDRColorData hdrColorData) where THDRColorData : struct, IHDRColorData
        {
            var hdrColorControl = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_Disp_HdrColorControl>();

            using (var hdrColorDataReference = ValueTypeReference.FromValueType(hdrColorData))
            {
                var status = hdrColorControl(displayId, hdrColorDataReference);

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return hdrColorDataReference.ToValueType<THDRColorData>().GetValueOrDefault();
            }
        }
    }
}