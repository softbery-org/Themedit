#!/bin/sh
_VERSION="Version `cat .sbver`"
echo ""
echo "Getting files ..."
echo ""
git add .
sleep 3s
echo ""
echo ""
echo "Commiting all files ...: $_VERSION"
echo ""
git commit -m "$_VERSION"
sleep 3s
echo ""
echo ""
echo "Pushing repository on Github server..."
echo ""
git push
echo ""
echo "All done ..."
echo "..."
echo ""