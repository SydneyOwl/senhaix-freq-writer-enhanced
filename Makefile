COMPLIER=msbuild

STAELLITE_JSON=$(CURDIR)/data/data/amsat-all-frequencies.json
GT12_SRCDIR=$(CURDIR)/GT12
GT12_DISTDIR=$(CURDIR)/GT12/bin/Debug/net462
GT12_DISTNAME=SHX-GT12.exe
SHX_SRCDIR=$(CURDIR)/shx8x00
SHX_NOBT_DISTDIR=$(CURDIR)/shx8x00/bin/Debug/net20
SHX_NOBT_DISTNAME=SHX8X00.exe
SHX_NOBT_SOLUTION_NAME=SHX8X00_nobt.csproj
SHX_BT_DISTDIR=$(CURDIR)/shx8x00/bin/Debug/net462
SHX_BT_DISTNAME=SHX8X00.exe
SHX_BT_SOLUTION_NAME=SHX8X00.csproj

all: shx8x00 shx8x00_nobt gt12

clean_gt12:
	$(COMPLIER) $(GT12_SRCDIR) /t:clean

restore_gt12:
	$(COMPLIER) $(GT12_SRCDIR) /t:restore

build_gt12:
	$(COMPLIER) $(GT12_SRCDIR) 

clean_shx8x00_nobt:
	$(COMPLIER) /t:clean $(SHX_SRCDIR)\$(SHX_NOBT_SOLUTION_NAME)

restore_shx8x00_nobt:
	$(COMPLIER) /t:restore $(SHX_SRCDIR)\$(SHX_NOBT_SOLUTION_NAME)

build_shx8x00_nobt:
	$(COMPLIER) $(SHX_SRCDIR)\$(SHX_NOBT_SOLUTION_NAME)

clean_shx8x00:
	$(COMPLIER) /t:clean $(SHX_SRCDIR)\$(SHX_BT_SOLUTION_NAME)

restore_shx8x00:
	$(COMPLIER) /t:restore $(SHX_SRCDIR)\$(SHX_BT_SOLUTION_NAME)

build_shx8x00:
	$(COMPLIER) $(SHX_SRCDIR)\$(SHX_BT_SOLUTION_NAME)

gt12: restore_gt12 clean_gt12 build_gt12

shx8x00_nobt: restore_shx8x00_nobt clean_shx8x00_nobt build_shx8x00_nobt 

shx8x00: restore_shx8x00 clean_shx8x00 build_shx8x00

.PHONY: clean_gt12 clean_shx8x00 clean_shx8x00_nobt restore_gt12 restore_shx8x00 restore_shx8x00_nobt build_gt12 build_shx8x00 build_shx8x00_nobt