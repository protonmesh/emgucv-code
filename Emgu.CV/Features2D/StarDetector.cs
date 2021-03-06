//----------------------------------------------------------------------------
//  Copyright (C) 2004-2014 by EMGU Corporation. All rights reserved.       
//----------------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.Util;

namespace Emgu.CV.Features2D
{
   /// <summary>
   /// StarDetector
   /// </summary>
   public class StarDetector : UnmanagedObject, IFeatureDetector
   {
      static StarDetector()
      {
         CvInvoke.CheckLibraryLoaded();
      }

      #region IFeatureDetector Members
      /// <summary>
      /// Get the feature detector. 
      /// </summary>
      /// <returns>The feature detector</returns>
      IntPtr IFeatureDetector.FeatureDetectorPtr
      {
         get
         {
            return _ptr;
         }
      }
      #endregion

      IntPtr IAlgorithm.AlgorithmPtr
      {
         get
         {
            return CvInvoke.cveAlgorithmFromFeatureDetector(((IFeatureDetector)this).FeatureDetectorPtr);
         }
      }

      /// <summary>
      /// Create a star detector with the specific parameters
      /// </summary>
      /// <param name="maxSize">
      /// Maximum size of the features. The following
      /// values of the parameter are supported:
      /// 4, 6, 8, 11, 12, 16, 22, 23, 32, 45, 46, 64, 90, 128</param>
      /// <param name="responseThreshold">
      /// Threshold for the approximated laplacian,
      /// used to eliminate weak features. The larger it is,
      /// the less features will be retrieved
      /// </param>
      /// <param name="lineThresholdProjected">
      /// Another threshold for the laplacian to eliminate edges.
      /// The larger the threshold, the more points you get.
      /// </param>
      /// <param name="lineThresholdBinarized">
      /// Another threshold for the feature size to eliminate edges. 
      /// The larger the threshold, the more points you get.</param>
      /// <param name="suppressNonmaxSize">
      ///
      /// </param>
      public StarDetector(int maxSize = 45, int responseThreshold = 30, int lineThresholdProjected = 10, int lineThresholdBinarized = 8, int suppressNonmaxSize = 5)
      {
         _ptr = CvStarDetectorCreate(maxSize, responseThreshold, lineThresholdProjected, lineThresholdBinarized, suppressNonmaxSize);
      }

      /// <summary>
      /// Release the unmanaged memory associated with this detector.
      /// </summary>
      protected override void DisposeObject()
      {
         if (_ptr != IntPtr.Zero)
            CvStarDetectorRelease(ref _ptr);
      }

      [DllImport(CvInvoke.ExternLibrary, CallingConvention = CvInvoke.CvCallingConvention)]
      internal extern static IntPtr CvStarDetectorCreate(int maxSize, int responseThreshold, int lineThresholdProjected, int lineThresholdBinarized, int suppressNonmaxSize);

      [DllImport(CvInvoke.ExternLibrary, CallingConvention = CvInvoke.CvCallingConvention)]
      internal extern static void CvStarDetectorRelease(ref IntPtr detector);

   }
}
