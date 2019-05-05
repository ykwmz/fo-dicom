﻿// Copyright (c) 2012-2019 fo-dicom contributors.
// Licensed under the Microsoft Public License (MS-PL).

using Dicom.Imaging.Codec.JpegLossless;

namespace Dicom.Imaging.Codec
{

    public class JpegLosslessDecoderWrapperProcess14 : DicomJpegLosslessDecoder
    {

        public override DicomTransferSyntax TransferSyntax => DicomTransferSyntax.JPEGProcess14;

    }

}
