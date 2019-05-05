﻿// Copyright (c) 2012-2019 fo-dicom contributors.
// Licensed under the Microsoft Public License (MS-PL).

namespace Dicom.Helpers
{
    using Dicom.Log;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Xunit.Abstractions;

    public class XUnitDicomLogger : Logger
    {
        private delegate string PrefixEnricher(string prefix);

        private readonly ITestOutputHelper _testOutputHelper;
        private readonly List<PrefixEnricher> _prefixEnrichers;
        private readonly LogLevel _minimumLevel;

        public XUnitDicomLogger(ITestOutputHelper testOutputHelper) : this(testOutputHelper, LogLevel.Debug, new List<PrefixEnricher>()) { }

        XUnitDicomLogger(ITestOutputHelper testOutputHelper, LogLevel minimumLevel, IEnumerable<PrefixEnricher> prefixEnrichers)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
            _minimumLevel = minimumLevel;
            _prefixEnrichers = prefixEnrichers?.ToList() ?? throw new ArgumentNullException(nameof(prefixEnrichers));
        }

        XUnitDicomLogger WithPrefixEnricher(PrefixEnricher prefixEnricher)
        {
            if (prefixEnricher == null) throw new ArgumentNullException(nameof(prefixEnricher));

            var prefixEnrichers = new List<PrefixEnricher>(_prefixEnrichers) {prefixEnricher};

            return new XUnitDicomLogger(_testOutputHelper, _minimumLevel, prefixEnrichers);
        }

        public XUnitDicomLogger IncludeThreadId() => WithPrefixEnricher(prefix => $"{prefix} #{System.Threading.Thread.CurrentThread.ManagedThreadId,3}");

        public XUnitDicomLogger IncludeTimestamps() => WithPrefixEnricher(prefix => $"{prefix} {DateTime.Now: HH:mm:ss.fff}");

        public XUnitDicomLogger IncludePrefix(string prefix) => WithPrefixEnricher(existingPrefix => $"{existingPrefix} {prefix, 25}");

        public XUnitDicomLogger WithMinimumLevel(LogLevel minimumLevel) => new XUnitDicomLogger(_testOutputHelper, minimumLevel, _prefixEnrichers);

        public override void Log(LogLevel level, string msg, params object[] args)
        {
            if (level < _minimumLevel)
                return;

            var prefix = _prefixEnrichers.Aggregate(
                $"{nameof(XUnitDicomLogger), 20} {level.ToString().ToUpper(), 6}",
                (intermediatePrefix, enrichPrefix) => enrichPrefix(intermediatePrefix));
            var message = string.Format(NameFormatToPositionalFormat(msg), args);
            var line = $"{prefix} : {message}";
            try
            {
                _testOutputHelper.WriteLine(line);
            }
            catch (Exception)
            {
                // Ignored, trying to log before or after tests cannot be handled properly
            }
        }
    }
}
