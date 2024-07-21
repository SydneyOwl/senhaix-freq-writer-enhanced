package logger

import (
	"github.com/gookit/slog"
)

var (
	logTemplate         = "[{{datetime}}] [{{level}}] {{message}}\n"
	logCodeLineTemplate = "[{{datetime}}] [{{level}}] {{message}} (from:{{caller}})\n"

	defaultLevels = slog.Levels{slog.NoticeLevel, slog.InfoLevel}
	verboseLevels  = slog.Levels{slog.NoticeLevel, slog.InfoLevel, slog.DebugLevel}
	vverboseLevels = slog.Levels{slog.NoticeLevel, slog.InfoLevel, slog.DebugLevel, slog.TraceLevel}
)

func InitLog(verbose bool, vverbose bool, colorDisable bool) {
	slog.Configure(func(l *slog.SugaredLogger) {
		f := l.Formatter.(*slog.TextFormatter)
		f.TimeFormat = "2006/01/02 15:04:05"
		f.EnableColor = !colorDisable
		if verbose || vverbose {
			f.SetTemplate(logCodeLineTemplate)
		} else {
			f.SetTemplate(logTemplate)
		}
		// slog config
	})
	logLevel := defaultLevels
	if verbose {
		logLevel = verboseLevels
	}
	if vverbose {
		logLevel = vverboseLevels
	}
	slog.SetLogLevel(logLevel[len(logLevel)-1])
}
