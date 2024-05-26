package logger

import (
	"github.com/gookit/slog"
)

var (
	logTemplate         = "[{{datetime}}] [{{level}}] {{message}}\n"
	logCodeLineTemplate = "[{{datetime}}] [{{level}}] {{message}} (from:{{caller}})\n"

	DefaultNormalLevels = slog.Levels{slog.NoticeLevel, slog.InfoLevel}
	VerboseLevels       = slog.Levels{slog.NoticeLevel, slog.InfoLevel, slog.DebugLevel}
	VVboseLevels        = slog.Levels{slog.NoticeLevel, slog.InfoLevel, slog.DebugLevel, slog.TraceLevel}
)

func InitLog(verbose bool, vverbose bool) {
	slog.Configure(func(l *slog.SugaredLogger) {
		f := l.Formatter.(*slog.TextFormatter)
		f.TimeFormat = "2006/01/02 15:04:05"
		if verbose || vverbose {
			f.SetTemplate(logCodeLineTemplate)
		} else {
			f.SetTemplate(logTemplate)
		}
		// slog config
	})
	logLevel := DefaultNormalLevels
	if verbose {
		logLevel = VerboseLevels
	}
	if vverbose {
		logLevel = VVboseLevels
	}
	slog.SetLogLevel(logLevel[len(logLevel)-1])
}
