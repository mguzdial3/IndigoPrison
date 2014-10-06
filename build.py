from __future__ import absolute_import, print_function, unicode_literals

import sys
import os
import subprocess
import argparse
import traceback
import platform
import timeit
import shutil
import json
import uuid
import datetime

localdir = os.path.abspath(os.path.dirname(__file__))
outdir = os.path.abspath(os.path.join(localdir, 'dist/'))
system = platform.system()

revid = subprocess.check_output(['hg', 'parent', '--template', '{node}'], cwd=localdir)
status = subprocess.check_output(['hg', 'status'])

def check(code):
    if code != 0:
        raise Exception('step failed')

def build_clean(mode):
    try:
        shutil.rmtree('dist/')
    except:
        traceback.print_exc()

# the F# library
def build_gamelib(args):
    slnfile = "external/Indigo/Indigo.csproj"

    if args.mode == 'dev':
        configarg = 'Debug'
        define = 'DEVEOLOPMENT'
    elif args.mode == 'prd':
        configarg = 'Release'
        define = 'PRODUCTION'
    else:
        raise ValueError("Invalid configuration '%s', aborting.\n" % args.mode)

    if system == 'Windows':
        command = "C:\\Program Files (x86)\\Xamarin Studio\\bin\\mdtool"
        config = "--configuration:" + configarg
        check(subprocess.call([command, "build", config, "--target:Clean", slnfile]))
        check(subprocess.call([command, "build", config, "--target:Build", slnfile]))
	# MacOS users please modify here
    elif system == 'Darwin':
        command = "/Applications/Xamarin Studio.app/Contents/MacOS/mdtool"
        config = "--configuration:" + configarg
        check(subprocess.call([command, "build", config, "--target:Clean", slnfile]))
        check(subprocess.call([command, "build", config, "--target:Build", slnfile]))
	# Linux users please add a following clause here
    else:
        raise Exception("Platform unsupported!")

# the Unity project
# XXX (kasiu): Currently uncommented for convenience.
# Basically, I stole this from another project and need to strip out some things.
# def build_unity(args):
    # unitydir = os.path.join(localdir, 'unity')
    # configfilename = os.path.join(unitydir, 'Assets/Resources/config.json')

    # try:
        # if status != '' and args.mode == 'prd':
            # raise ValueError("Cannot create production build on unclean repository! Please commit your changes or revert to a clean version.")

        # namespace = uuid.UUID('fb65c2fe-36a4-4a42-97a1-afe8d41177ac')

        # config = {
            # 'id': str(uuid.uuid5(namespace, revid)),
            # 'revision': revid,
            # 'buildtime': datetime.datetime.utcnow().isoformat(b' ')
        # }

        # with open(configfilename, 'w') as f:
            # json.dump(config, f)

        # if system == 'Windows':
            # unityExecutable = "C:\\Program Files (x86)\\Unity\\Editor\\Unity.exe"
        # elif system == 'Darwin':
            # unityExecutable = "/Applications/Unity/Unity.app/Contents/MacOS/Unity"
        # else:
            # print('platform unsupported!')
            # sys.exit(1)

        # print('building unity project in "%s", outputting to "%s"...' % (unitydir, outdir))

        # subprocess.call([
            # unityExecutable,
            # "-batchmode",
            # "-projectPath",
            # unitydir,
            # "-buildWindowsPlayer",
            # os.path.join(outdir, "win/indigo_prison.exe"),
            # "-buildOSXUniversalPlayer",
            # os.path.join(outdir, "osx/indigo_prison.exe"),
            # "-quit",
        # ], cwd=localdir)
    # finally:
        # # always remove temp files, even on failure
        # try:
            # os.remove(configfilename)
            # os.remove(configfilename + '.meta')
        # except: pass

build_steps = {
    'gamelib': build_gamelib,
    # 'unity': build_unity,
    'clean': build_clean,
}

def build(target, args):
    print("Building '%s'..." % target)
    build_steps[target](args)
    print("Done building '%s'." % target)

def main(args):

    start_time = timeit.default_timer()

    build_order = {
        #'all': ['gamelib', 'unity'],
		'all': ['gamelib'],
    }
    target = args.target

    try:
        if target in build_order:
            for t in build_order[target]: build(t, args)
        else:
            build(target, args)
    except:
        traceback.print_exc()
        sys.stderr.write("BUILD FAILED :(\n")
        sys.exit(1)

    end_time = timeit.default_timer()
    print('Finished in %f seconds' % (end_time - start_time))

if __name__ == '__main__':
    _desc = '''
    Build all the things.
    Does not do dependency checking, simply builds them all in order.
    You can only build one step of the process by passing in something other than 'all'.
    '''
    targets = ['all', 'gamelib', 'unity', 'clean']

    parser = argparse.ArgumentParser(description=_desc)
    parser.add_argument('target', nargs='?', default='all', choices=targets, help='The target to build. Defaults to all.')

    mut = parser.add_mutually_exclusive_group()
    mut.add_argument('-d', action='store_const', dest='mode', const='dev', default='dev',
        help="Build in development mode (default)."
    )
    mut.add_argument('-p', action='store_const', dest='mode', const='prd',
        help="Build in production mode."
    )

    main(parser.parse_args())

