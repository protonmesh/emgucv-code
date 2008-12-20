using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using NUnit.Framework;
using Emgu.CV.Structure;

namespace Emgu.CV.ML.UnitTest
{
   [TestFixture]
   public class UnitTests
   {
      [Test]
      public void TestKNearest()
      {
         int K = 10;
         int trainSampleCount = 100;

         #region Generate the traning data and classes

         Matrix<float> trainData = new Matrix<float>(trainSampleCount, 2);
         Matrix<float> trainClasses = new Matrix<float>(trainSampleCount, 1);

         Image<Bgr, Byte> img = new Image<Bgr, byte>(500, 500);

         Matrix<float> sample = new Matrix<float>(1, 2);

         Matrix<float> trainData1 = trainData.GetRows(0, trainSampleCount >> 1, 1);
         trainData1.SetRandNormal(new MCvScalar(200), new MCvScalar(50));
         Matrix<float> trainData2 = trainData.GetRows(trainSampleCount >> 1, trainSampleCount, 1);
         trainData2.SetRandNormal(new MCvScalar(300), new MCvScalar(50));

         Matrix<float> trainClasses1 = trainClasses.GetRows(0, trainSampleCount >> 1, 1);
         trainClasses1.SetValue(1);
         Matrix<float> trainClasses2 = trainClasses.GetRows(trainSampleCount >> 1, trainSampleCount, 1);
         trainClasses2.SetValue(2);
         #endregion
         
         Matrix<float> results, neighborResponses;
         results = new Matrix<float>(sample.Rows, 1);
         neighborResponses = new Matrix<float>(sample.Rows, K);
         //dist = new Matrix<float>(sample.Rows, K);

         using (KNearest knn = new KNearest(trainData, trainClasses, null, false, K))
         {
            for (int i = 0; i < img.Height; i++)
            {
               for (int j = 0; j < img.Width; j++)
               {
                  sample.Data[0, 0] = j;
                  sample.Data[0, 1] = i;

                  //Matrix<float> nearestNeighbors = new Matrix<float>(K* sample.Rows, sample.Cols);
                  // estimates the response and get the neighbors' labels
                  float response = knn.FindNearest(sample, K, results, null, neighborResponses, null);

                  int accuracy = 0;
                  // compute the number of neighbors representing the majority
                  for (int k = 0; k < K; k++)
                  {
                     if (neighborResponses.Data[0, k] == response)
                        accuracy++;
                  }
                  // highlight the pixel depending on the accuracy (or confidence)
                  img[i, j] =
                  response == 1 ?
                      (accuracy > 5 ? new Bgr(90, 0, 0) : new Bgr(90, 60, 0)) :
                      (accuracy > 5 ? new Bgr(0, 90, 0) : new Bgr(60, 90, 0));
               }
            }
         }

         // display the original training samples
         for (int i = 0; i < (trainSampleCount >> 1); i++)
         {
            PointF p1 = new PointF(trainData1[i, 0], trainData1[i, 1]);
            img.Draw(new CircleF(p1, 2.0f), new Bgr(255, 100, 100), -1);
            PointF p2 = new PointF(trainData2[i, 0], trainData2[i, 1]);
            img.Draw(new CircleF(p2, 2.0f), new Bgr(100, 255, 100), -1);
         }

         Emgu.CV.UI.ImageViewer.Show(img);
      }

      [Test]
      public void TestEM()
      {
         int N = 4; //number of clusters
         int N1 = (int)Math.Sqrt((double)4);

         Bgr[] colors = new Bgr[] { 
            new Bgr(0, 0, 255), 
            new Bgr(0, 255, 0),
            new Bgr(0, 255, 255),
            new Bgr(255, 255, 0)};

         int nSamples = 100;

         Matrix<float> samples = new Matrix<float>(nSamples, 2);
         Matrix<Int32> labels = new Matrix<int>(nSamples, 1);
         Image<Bgr, Byte> img = new Image<Bgr,byte>(500, 500);
         Matrix<float> sample = new Matrix<float>(1, 2);

         CvInvoke.cvReshape(samples.Ptr, samples.Ptr, 2, 0);
         for (int i = 0; i < N; i++)
         {
            Matrix<float> rows = samples.GetRows(i * nSamples / N, (i + 1) * nSamples / N, 1);
            double scale = ((i % N1) + 1.0) / (N1 + 1);
            MCvScalar mean = new MCvScalar(scale * img.Width, scale * img.Height);
            MCvScalar sigma = new MCvScalar(30, 30);
            ulong seed = (ulong)DateTime.Now.Ticks;
            CvInvoke.cvRandArr(ref seed, rows.Ptr, Emgu.CV.CvEnum.RAND_TYPE.CV_RAND_NORMAL, mean, sigma);
         }
         CvInvoke.cvReshape(samples.Ptr, samples.Ptr, 1, 0);

         using (EM emModel1 = new EM())
         using (EM emModel2 = new EM())
         {
            EMParams parameters1 = new EMParams();
            parameters1.Nclusters = N;
            parameters1.CovMatType = Emgu.CV.ML.MlEnum.EM_COVARIAN_MATRIX_TYPE.COV_MAT_DIAGONAL;
            parameters1.StartStep = Emgu.CV.ML.MlEnum.EM_INIT_STEP_TYPE.START_AUTO_STEP;
            parameters1.TermCrit = new MCvTermCriteria(10, 0.01);
            emModel1.Train(samples, null, parameters1, labels);

            EMParams parameters2 = new EMParams();
            parameters2.Nclusters = N;
            parameters2.CovMatType = Emgu.CV.ML.MlEnum.EM_COVARIAN_MATRIX_TYPE.COV_MAT_GENERIC;
            parameters2.StartStep = Emgu.CV.ML.MlEnum.EM_INIT_STEP_TYPE.START_E_STEP;
            parameters2.TermCrit = new MCvTermCriteria(100, 1.0e-6);
            parameters2.Means = emModel1.GetMeans();
            parameters2.Covs = emModel1.GetCovariances();
            parameters2.Weights = emModel1.GetWeights();
                        
            emModel2.Train(samples, null, parameters2, labels);
            
            #region Classify every image pixel
            for (int i = 0; i < img.Height; i++)
               for (int j = 0; j < img.Width; j++)
               {
                  sample.Data[0, 0] = i;
                  sample.Data[0, 1] = j;
                  int response = (int) emModel2.Predict(sample, null);

                  Bgr color = colors[response];
                  
                  img.Draw(
                     new CircleF(new PointF(j, i), 1), 
                     new Bgr(color.Blue*0.5, color.Green * 0.5, color.Red * 0.5 ), 
                     0);
               }
            #endregion 

            #region draw the clustered samples
            for (int i = 0; i < nSamples; i++)
            {
               img.Draw(new CircleF(new PointF(samples.Data[i, 0], samples.Data[i, 1]), 1), colors[labels.Data[i, 0]], 0);
            }
            #endregion 
   
            //Emgu.CV.UI.ImageViewer.Show(img);
         }
      }

      [Test]
      public void TestSVM()
      {
         using (SVM model = new SVM())
         {
         }
      }

      [Test]
      public void TestNormalBayesClassifier()
      {
         using (NormalBayesClassifier classifier = new NormalBayesClassifier())
         {
         }
      }

      [Test]
      public void TestANN_MLP()
      {
         int trainSampleCount = 100;

         #region Generate the traning data and classes
         Matrix<float> trainData = new Matrix<float>(trainSampleCount, 2);
         Matrix<float> trainClasses = new Matrix<float>(trainSampleCount, 1);

         Image<Bgr, Byte> img = new Image<Bgr, byte>(500, 500);

         Matrix<float> sample = new Matrix<float>(1, 2);
         Matrix<float> prediction = new Matrix<float>(1, 1);

         Matrix<float> trainData1 = trainData.GetRows(0, trainSampleCount >> 1, 1);
         trainData1.SetRandNormal(new MCvScalar(200), new MCvScalar(50));
         Matrix<float> trainData2 = trainData.GetRows(trainSampleCount >> 1, trainSampleCount, 1);
         trainData2.SetRandNormal(new MCvScalar(300), new MCvScalar(50));

         Matrix<float> trainClasses1 = trainClasses.GetRows(0, trainSampleCount >> 1, 1);
         trainClasses1.SetValue(1);
         Matrix<float> trainClasses2 = trainClasses.GetRows(trainSampleCount >> 1, trainSampleCount, 1);
         trainClasses2.SetValue(2);
         #endregion

         Matrix<int> layerSize = new Matrix<int>(new int[] { 2, 5, 1 });

         MCvANN_MLP_TrainParams parameters = new MCvANN_MLP_TrainParams();
         parameters.term_crit = new MCvTermCriteria(10, 1.0e-8);
         parameters.train_method = Emgu.CV.ML.MlEnum.ANN_MLP_TRAIN_METHOD.BACKPROP;
         parameters.bp_dw_scale = 0.1;
         parameters.bp_moment_scale = 0.1;

         using (ANN_MLP network = new ANN_MLP(layerSize, Emgu.CV.ML.MlEnum.ANN_MLP_ACTIVATION_FUNCTION.SIGMOID_SYM, 1.0, 1.0))
         {
            network.Train(trainData, trainClasses, null, null, parameters, Emgu.CV.ML.MlEnum.ANN_MLP_TRAINING_FLAG.DEFAULT);

            for (int i = 0; i < img.Height; i++)
            {
               for (int j = 0; j < img.Width; j++)
               {
                  sample.Data[0, 0] = j;
                  sample.Data[0, 1] = i;
                  network.Predict(sample, prediction);

                  // estimates the response and get the neighbors' labels
                  float response = prediction.Data[0,0];

                  // highlight the pixel depending on the accuracy (or confidence)
                  img[i, j] = response < 1.5 ? new Bgr(90, 0, 0) : new Bgr(0, 90, 0);
               }
            }
         }

         // display the original training samples
         for (int i = 0; i < (trainSampleCount >> 1); i++)
         {
            PointF p1 = new PointF(trainData1[i, 0], trainData1[i, 1]);
            img.Draw(new CircleF(p1, 2), new Bgr(255, 100, 100), -1);
            PointF p2 = new PointF((int)trainData2[i, 0], (int)trainData2[i, 1]);
            img.Draw(new CircleF(p2, 2), new Bgr(100, 255, 100), -1);
         }
         //Emgu.CV.UI.ImageViewer.Show(img);
      }
   }
}